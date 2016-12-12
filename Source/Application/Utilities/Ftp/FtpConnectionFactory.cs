namespace pdfforge.PDFCreator.Utilities.Ftp
{
    public interface IFtpConnectionFactory
    {
        IFtpConnection BuilConnection(string host, string userName, string password);
    }

    public class FtpConnectionFactory : IFtpConnectionFactory
    {
        public IFtpConnection BuilConnection(string host, string userName, string password)
        {
            return new FtpConnectionWrap(host, userName, password);
        }
    }
}