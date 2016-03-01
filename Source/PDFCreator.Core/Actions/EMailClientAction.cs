using System;
using NLog;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Mail;

namespace pdfforge.PDFCreator.Core.Actions
{
    public class EMailClientAction : IAction
    {
        private readonly string _signatureText;
        private const int ActionId = 11;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEmailClientFactory _emailClientFactory = new EmailClientFactory();

        public EMailClientAction(string signatureText)
        {
            _signatureText = signatureText;
        }

        public EMailClientAction(IEmailClientFactory emailClientFactory, string signatureText)
        {
            _emailClientFactory = emailClientFactory;
            _signatureText = signatureText;
        }

        public static bool CheckEmailClientInstalled()
        {
            var emailClientFactory = new EmailClientFactory();
            return emailClientFactory.CreateEmailClient() != null;
        }

        public ActionResult ProcessJob(IJob job)
        {
            try
            {
                _logger.Info("Launched client e-mail action");

                var message = new Email();

                message.Subject = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClient.Subject);
                message.Body = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClient.Content);

                if (job.Profile.EmailClient.AddSignature)
                {
                    message.Body += _signatureText;
                }

                foreach (string recipient in job.Profile.EmailClient.Recipients.Replace(',', ';').Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(recipient))
                        message.To.Add(recipient.Trim());
                }

                foreach (string file in job.OutputFiles)
                {
                    message.Attachments.Add(new Attachment(file));
                }

                _logger.Info("Starting e-mail client");

                var mailClient = _emailClientFactory.CreateEmailClient();

                if (mailClient == null)
                {
                    _logger.Error("No compatible e-mail client installed");
                    return new ActionResult(ActionId, 101);
                }

                bool success = mailClient.ShowEmailClient(message);

                if (!success)
                {
                    _logger.Warn("Could not start e-mail client");
                    return new ActionResult(ActionId, 100);
                }

                _logger.Info("Done starting e-mail client");
                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in e-mail client Action \r\n" + ex.Message);
                return new ActionResult(ActionId, 999);
            }
        }

        
    }
}