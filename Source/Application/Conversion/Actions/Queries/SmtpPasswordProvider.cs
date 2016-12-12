using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Actions.Queries
{
    public class SmtpPasswordProvider : ISmtpPasswordProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool SetPassword(Job job)
        {
            job.Passwords.SmtpPassword = job.Profile.EmailSmtpSettings.Password;
            return true;
        }

        public ActionResult RetypePassword(Job job)
        {
            Logger.Error("SendMailOverSmtp canceled. No Retype Smtp Password specified.");
            return new ActionResult(ErrorCode.Smtp_AuthenticationDenied);
        }
    }
}
