using FtpLib;

namespace pdfforge.PDFCreator.Utilities.Ftp
{
    public interface IFtpConnection
    {
        void Open();

        void Close();

        void Login();

        bool FileExists(string filePath);

        bool DirectoryExists(string path);

        void CreateDirectory(string path);

        void SetCurrentDirectory(string directory);

        void PutFile(string file, string path);
    }

    public class FtpConnectionWrap : IFtpConnection
    {
        private readonly FtpConnection _ftpConnection;

        public FtpConnectionWrap(string host, string userName, string password)
        {
            _ftpConnection = new FtpConnection(host, userName, password);
        }

        public void Open()
        {
            _ftpConnection.Open();
        }

        public void Close()
        {
            _ftpConnection.Close();
        }

        public void Login()
        {
            _ftpConnection.Login();
        }

        public bool FileExists(string filePath)
        {
            return _ftpConnection.FileExists(filePath);
        }

        public bool DirectoryExists(string path)
        {
            return _ftpConnection.DirectoryExists(path);
        }

        public void CreateDirectory(string path)
        {
            _ftpConnection.CreateDirectory(path);
        }

        public void SetCurrentDirectory(string directory)
        {
            _ftpConnection.SetCurrentDirectory(directory);
        }

        public void PutFile(string localFile, string remoteFile)
        {
            _ftpConnection.PutFile(localFile, remoteFile);
        }
    }
}
