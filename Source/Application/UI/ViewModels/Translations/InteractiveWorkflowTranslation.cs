using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class InteractiveWorkflowTranslation : ITranslatable
    {
        public string AnErrorOccured { get; private set; } = "An error occurred:";
        public string Error { get; private set; } = "Error";
        public string JpegFile { get; private set; } = "JPEG file";
        public string PdfA1bFile { get; private set; } = "PDF/A-1b file";
        public string PdfA2bFile { get; private set; } = "PDF/A-2b file";
        public string PdfFile { get; private set; } = "PDF file";
        public string PdfXFile { get; private set; } = "PDF/X file";
        public string PngFile { get; private set; } = "PNG file";
        public string RetypeFilenameMessage { get; private set; } = "The file could not be saved. Maybe the file is currently in use or you do not have the required permissions.\r\nPlease select another filename and try again.";
        public string RetypeSmtpPwMessage { get; private set; } = "Could not authenticate at server.\r\nPlease check your password and verify that you have a working internet connection.";
        public string SelectDestination { get; private set; } = "Select destination";
        public string SelectedPathTooLong { get; private set; } = "The total length of path and filename is too long.\r\nPlease use a shorter name.";
        public string TextFile { get; private set; } = "Text file";
        public string TiffFile { get; private set; } = "TIFF file";
        public string PasswordDescription { get; private set; } = "FTP server password:";
        public string PasswordTitle { get; private set; } = "FTP server password";
    }
}
