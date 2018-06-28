using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Net;
using System.Net.Mail;
using Attachment = System.Net.Mail.Attachment;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class SmtpMailAction : RetypePasswordActionBase, ISmtpMailAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override string PasswordText => "SMTP";

        public override ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();

            if (!profile.EmailSmtpSettings.Enabled)
                return actionResult;

            var smtpAccount = accounts.GetSmtpAccount(profile);

            return Check(profile, smtpAccount);
        }

        private ActionResult Check(ConversionProfile profile, SmtpAccount smtpAccount)
        {
            var actionResult = new ActionResult();

            if (smtpAccount == null)
            {
                Logger.Error($"The specified SMTP account with ID \"{profile.EmailSmtpSettings.AccountId}\" is not configured.");
                actionResult.Add(ErrorCode.Smtp_NoAccount);
                return actionResult;
            }

            if (string.IsNullOrWhiteSpace(smtpAccount.Address))
            {
                Logger.Error("No SMTP email address is specified.");
                actionResult.Add(ErrorCode.Smtp_NoEmailAddress);
            }
            if (string.IsNullOrWhiteSpace(profile.EmailSmtpSettings.Recipients))
            {
                Logger.Error("No SMTP email recipients are specified.");
                actionResult.Add(ErrorCode.Smtp_NoRecipients);
            }
            if (string.IsNullOrWhiteSpace(smtpAccount.Server))
            {
                Logger.Error("No SMTP host is specified.");
                actionResult.Add(ErrorCode.Smtp_NoServerSpecified);
            }

            if (smtpAccount.Port < 0)
            {
                Logger.Error("Invalid SMTP port.");
                actionResult.Add(ErrorCode.Smtp_InvalidPort);
            }

            if (string.IsNullOrWhiteSpace(smtpAccount.UserName))
            {
                Logger.Error("No SMTP UserName is specified.");
                actionResult.Add(ErrorCode.Smtp_NoUserSpecified);
            }

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrWhiteSpace(smtpAccount.Password))
                {
                    Logger.Error("No SMTP password for automatic saving.");
                    actionResult.Add(ErrorCode.Smtp_NoPasswordSpecified);
                }
            }

            return actionResult;
        }

        protected override ActionResult DoActionProcessing(Job job)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(job.Profile);

            var actionResult = Check(job.Profile, smtpAccount);
            if (!actionResult)
            {
                Logger.Error("Canceled SMTP mail action.");
                return actionResult;
            }

            if (string.IsNullOrEmpty(job.Passwords.SmtpPassword))
            {
                Logger.Error("SendMailOverSmtp canceled. Action launched without Password.");
                return new ActionResult(ErrorCode.Smtp_NoPasswordSpecified);
            }

            var recipientsTo = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Recipients.Replace(';', ','));
            var recipientsCc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.RecipientsCc.Replace(';', ','));
            var recipientsBcc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.RecipientsBcc.Replace(';', ','));

            MailMessage mail;

            try
            {
                mail = new MailMessage(smtpAccount.Address, recipientsTo);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                Logger.Error($"\'{recipientsTo}\' is no valid SMTP e-mail recipient: " + e.Message);
                return new ActionResult(ErrorCode.Smtp_InvalidRecipients);
            }

            // these blocks have to be seperated, because we want to log the offending recipients
            // (AddRecipients does this already, but can't be reused for the constructor)
            try
            {
                AddRecipients(mail, RecipientType.Cc, recipientsCc);
                AddRecipients(mail, RecipientType.Bcc, recipientsBcc);
            }
            catch
            {
                return new ActionResult(ErrorCode.Smtp_InvalidRecipients);
            }

            mail.Subject = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Subject);
            mail.IsBodyHtml = job.Profile.EmailSmtpSettings.Html;
            mail.Body = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Content);

            if (job.Profile.EmailSmtpSettings.AddSignature)
            {
                // if html option is checked replace newLine with <br />
                mail.Body += job.Profile.EmailSmtpSettings.Html ? job.JobTranslations.EmailSignature.Replace(Environment.NewLine, "<br>") : job.JobTranslations.EmailSignature;
            }

            Logger.Debug("Created new Mail"
                         + "\r\nFrom: " + mail.From
                         + "\r\nTo: " + mail.To
                         + "\r\nSubject: " + mail.Subject
                         + "\r\nContent: " + mail.Body
            );

            if (!SkipFileAttachments(job))
            {
                var i = 1;
                foreach (var file in job.OutputFiles)
                {
                    var attach = new Attachment(file);
                    //attach.NameEncoding = System.Text.Encoding.ASCII;
                    mail.Attachments.Add(attach);
                    Logger.Debug("Attachement " + i + "/" + job.OutputFiles.Count + ":" + file);
                    i++;
                }
            }

            var smtp = new SmtpClient(smtpAccount.Server, smtpAccount.Port);
            smtp.EnableSsl = smtpAccount.Ssl;

            Logger.Debug("Created new SmtpClient:"
                         + "\r\nHost: " + smtp.Host
                         + "\r\nPort: " + smtp.Port
            );

            return SendEmail(job, smtp, mail);
        }

        private MailAddressCollection GetAddressCollection(MailMessage mail, RecipientType recipientType)
        {
            switch (recipientType)
            {
                case RecipientType.To: return mail.To;
                case RecipientType.Cc: return mail.CC;
                case RecipientType.Bcc: return mail.Bcc;
                default: throw new ArgumentException("Invalid recipient type!", nameof(recipientType));
            }
        }

        private void AddRecipients(MailMessage mail, RecipientType recipientType, string recipients)
        {
            if (string.IsNullOrWhiteSpace(recipients))
                return;

            var addressCollection = GetAddressCollection(mail, recipientType);
            try
            {
                addressCollection.Add(recipients);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                Logger.Error($"\'{recipients}\' is no valid SMTP e-mail recipient: " + e.Message);
                throw;
            }
        }

        private bool SkipFileAttachments(Job job)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            return job.Profile.EmailSmtpSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private ActionResult SendEmail(Job job, SmtpClient smtp, MailMessage mail)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(job.Profile);

            var credentials = new NetworkCredential(smtpAccount.UserName, job.Passwords.SmtpPassword);
            smtp.Credentials = credentials;

            try
            {
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Logger.Error("The message could not be delivered to one or more of the recipients\r\n" + ex.Message);
                return new ActionResult(ErrorCode.Smtp_EmailNotDelivered);
            }
            catch (SmtpException ex)
            {
                Logger.Warn("Could not authorize to host.\r\n" + ex.Message);
                return new ActionResult(ErrorCode.PasswordAction_Login_Error);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while sending mail over smtp:\r\n" + ex.Message);

                return new ActionResult(ErrorCode.Smtp_GenericError);
            }
            finally
            {
                mail.Dispose();
            }

            return new ActionResult();
        }

        protected override void SetPassword(Job job, string password)
        {
            job.Passwords.SmtpPassword = password;
        }

        public override bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailSmtpSettings.Enabled;
        }
    }
}
