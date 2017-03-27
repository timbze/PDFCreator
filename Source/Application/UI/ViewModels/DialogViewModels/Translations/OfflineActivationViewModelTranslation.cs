using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
    public class OfflineActivationViewModelTranslation : ITranslatable
    {
        public string InvalidLicenseKeySyntax { get; private set; } = "Wrong license key syntax.\r\nIt must look like '12345-67890-ABCDE-EFGHI-JKLMNO-PRSTU'.\r\nValid characters are: A-Z, 0-9 and the dash.";
        public string CancelButtonContent { get; private set; } = "_Cancel";
        public string LicenseKeyLabelContent { get; private set; } = "License key";
        public string OkButtonContent { get; private set; } = "_OK";
        public string PasteResponseLabelContent { get; private set; } = "Paste the response code here:";
        public string RequestActivationCodeLabelContent { get; private set; } = "Request an activation code on our website:";
        public string Title { get; private set; } = "Offline activation";
    }
}
