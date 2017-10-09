using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PdfPasswordTranslation : ITranslatable
    {
        public string OwnerPasswordLabelContent { get; protected set; } = "Owner password (for editing):";
        public string SecurityTitle { get; protected set; } = "Security";
        public string UserPasswordLabelContent { get; protected set; } = "User password (for opening):";
    }

    public class PrintJobPasswordButtonTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string OkButtonContent { get; private set; } = "OK";
        public string RemoveButtonContent { get; private set; } = "Remove";
        public string SkipButtonContent { get; private set; } = "Skip";
    }
}
