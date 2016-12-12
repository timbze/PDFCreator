using System;
using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Mail;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class EMailClientAction : IAction
    {
        private readonly IEmailClientFactory _emailClientFactory;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public EMailClientAction(IEmailClientFactory emailClientFactory)
        {
            _emailClientFactory = emailClientFactory;
        }

        public ActionResult ProcessJob(Job job)
        {
            try
            {
                _logger.Info("Launched client e-mail action");

                var message = new Email();  

                message.Subject = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Subject);
                message.Body = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Content);

                if (job.Profile.EmailClientSettings.AddSignature)
                {
                    message.Body += job.JobTranslations.EmailSignature;
                }

                var recipients = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Recipients);
                foreach (var recipient in recipients.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        message.To.Add(recipient.Trim());
                }

                if (!SkipFileAttachments(job))
                {
                    foreach (var file in job.OutputFiles)
                    {
                        message.Attachments.Add(new Attachment(file));
                    }
                }

                _logger.Info("Starting e-mail client");

                var mailClient = _emailClientFactory.CreateEmailClient();

                if (mailClient == null)
                {
                    _logger.Error("No compatible e-mail client installed");
                    return new ActionResult(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);
                }

                var success = mailClient.ShowEmailClient(message);

                if (!success)
                {
                    _logger.Warn("Could not start e-mail client");
                    return new ActionResult(ErrorCode.MailClient_GenericError);
                }

                _logger.Info("Done starting e-mail client");
                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in e-mail client Action \r\n" + ex.Message);
                return new ActionResult(ErrorCode.MailClient_GenericError);
            }
        }

        private bool SkipFileAttachments(Job job)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            return job.Profile.EmailClientSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailClientSettings.Enabled;
        }

        public bool Init(Job job)
        {
            return true;
        }

        public bool CheckEmailClientInstalled()
        {
            return _emailClientFactory.CreateEmailClient() != null;
        }
    }
}