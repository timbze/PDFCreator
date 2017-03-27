using System;
using System.Net;
using System.Net.Mail;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class SmtpMailAction : IAction, ICheckable, ISmtpMailAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ISmtpPasswordProvider _passwordProvider;

        public SmtpMailAction(ISmtpPasswordProvider passwordProvider)
        {
            _passwordProvider = passwordProvider;
        }

        /// <summary>
        ///     Sends the created files using SMTP
        /// </summary>
        /// <param name="job"></param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
        {
            Logger.Debug("Launched smtp mail action");
            try
            {
                var result = SendMailOverSmtp(job);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in smtp mail action:\r\n" + ex.Message);
                return new ActionResult(ErrorCode.Smtp_GenericError);
            }
        }

        public bool Init(Job job)
        {
            return _passwordProvider.SetPassword(job);
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailSmtpSettings.Enabled;
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();

            if (profile.EmailSmtpSettings.Enabled)
            {
                if (string.IsNullOrEmpty(profile.EmailSmtpSettings.Address))
                {
                    Logger.Error("No SMTP email address is specified.");
                    actionResult.Add(ErrorCode.Smtp_NoEmailAddress);
                }
                if (string.IsNullOrEmpty(profile.EmailSmtpSettings.Recipients))
                {
                    Logger.Error("No SMTP email recipients are specified.");
                    actionResult.Add(ErrorCode.Smtp_NoRecipients);
                }
                if (string.IsNullOrEmpty(profile.EmailSmtpSettings.Server))
                {
                    Logger.Error("No SMTP host is specified.");
                    actionResult.Add(ErrorCode.Smtp_NoServerSpecified);
                }

                if (profile.EmailSmtpSettings.Port < 0)
                {
                    Logger.Error("Invalid SMTP port.");
                    actionResult.Add(ErrorCode.Smtp_InvalidPort);
                }

                if (string.IsNullOrEmpty(profile.EmailSmtpSettings.UserName))
                {
                    Logger.Error("No SMTP UserName is specified.");
                    actionResult.Add(ErrorCode.Smtp_NoUserSpecified);
                }

                if (profile.AutoSave.Enabled)
                {
                    if (string.IsNullOrEmpty(profile.EmailSmtpSettings.Password))
                    {
                        Logger.Error("No SMTP password for automatic saving.");
                        actionResult.Add(ErrorCode.Smtp_NoPasswordSpecified);
                    }
                }
            }

            return actionResult;
        }

        private ActionResult SendMailOverSmtp(Job job)
        {
            var actionResult = Check(job.Profile, job.Accounts);
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

            var recipients = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Recipients);
            recipients = recipients.Replace(';', ',');

            var mail = new MailMessage(job.Profile.EmailSmtpSettings.Address, recipients);
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

            var smtp = new SmtpClient(job.Profile.EmailSmtpSettings.Server, job.Profile.EmailSmtpSettings.Port);
            smtp.EnableSsl = job.Profile.EmailSmtpSettings.Ssl;

            Logger.Debug("Created new SmtpClient:"
                         + "\r\nHost: " + smtp.Host
                         + "\r\nPort: " + smtp.Port
                );

            return SendEmail(job, smtp, mail);
        }

        private bool SkipFileAttachments(Job job)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            return job.Profile.EmailSmtpSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private ActionResult SendEmail(Job job, SmtpClient smtp, MailMessage mail)
        {
            var credentials = new NetworkCredential(job.Profile.EmailSmtpSettings.UserName, job.Passwords.SmtpPassword);
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

                var retypeResult = _passwordProvider.RetypePassword(job);
                if (retypeResult)
                {
                    return SendEmail(job, smtp, mail);
                }
                return retypeResult;
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
    }
}