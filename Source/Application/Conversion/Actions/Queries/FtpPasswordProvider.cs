using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Actions.Queries
{
    public class FtpPasswordProvider : IFtpPasswordProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool SetPassword(Job job)
        {
            job.Passwords.FtpPassword = job.Profile.Ftp.Password;
            return true;
        }

        public ActionResult RetypePassword(Job job)
        {
            Logger.Error("FtpUpload canceled. No Retype Ftp Password specified.");
            return new ActionResult(ErrorCode.Smtp_AuthenticationDenied);
        }
    }
}
