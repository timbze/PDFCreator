using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class HttpPasswordTranslation : ITranslatable
    {
        public string HttpUploadTitle { get; private set; } = "HTTP Upload";
        public string Account { get; private set; } = "HTTP Account:";
        public string Password { get; private set; } = "HTTP Server Password:";
    }
}
