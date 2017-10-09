using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class PrinterTabTranslation : ITranslatable
    {
        public string AddPrinterButtonText { get; private set; } = "Add Printer";
        public string DeletePrinterButtonText { get; private set; } = "Delete Printer";
        public string LastUsedProfileMapping { get; private set; } = "Last used profile";
        public string PrimaryColumnHeader { get; private set; } = "Primary Printer";
        public string PrinterColumnHeader { get; private set; } = "Printer";
        public string ProfileColumnHeader { get; private set; } = "Profile";
        public string RenamePrinterButtonText { get; private set; } = "Rename Printer";
        public string ManagePrinters { get; private set; } = "Manage Printers";
    }
}
