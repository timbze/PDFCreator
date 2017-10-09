namespace pdfforge.PDFCreator.Utilities.Ftp
{
    public interface IFtpConnectionFactory
    {
        IFtpConnection BuildConnection(string host, string userName, string password);
    }

    public class FtpConnectionFactory : IFtpConnectionFactory
    {
        public IFtpConnection BuildConnection(string host, string userName, string password)
        {
            return new FtpConnectionWrap(host, userName, password);
        }
    }
}
