using pdfforge.PDFCreator.Utilities.Ftp;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public class UniqueFilenameForFtp : UniqueFilenameBase
    {
        private readonly IFtpConnection _ftpConnection;

        public UniqueFilenameForFtp(string originalFilename, IFtpConnection ftpConnection, IPathUtil pathUtil)
            : base(originalFilename, pathUtil)
        {
            _ftpConnection = ftpConnection;
        }

        protected override bool UniqueCondition(string file)
        {
            return _ftpConnection.FileExists(file);
        }
    }
}