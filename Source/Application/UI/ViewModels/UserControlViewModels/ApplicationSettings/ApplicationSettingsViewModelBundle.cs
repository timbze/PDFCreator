namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class ApplicationSettingsViewModelBundle
    {
        public ApplicationSettingsViewModelBundle(GeneralTabViewModel generalTabViewModel,
            PrinterTabViewModel printerTabViewModel, TitleTabViewModel titleTabViewModel,
            DebugTabViewModel debugTabViewModel, LicenseTabViewModel licenseTabViewModel, PdfArchitectTabViewModel pdfArchitectTabViewModel)
        {
            GeneralTabViewModel = generalTabViewModel;
            PrinterTabViewModel = printerTabViewModel;
            TitleTabViewModel = titleTabViewModel;
            DebugTabViewModel = debugTabViewModel;
            LicenseTabViewModel = licenseTabViewModel;
            PdfArchitectTabViewModel = pdfArchitectTabViewModel;
        }

        public LicenseTabViewModel LicenseTabViewModel { get; }

        public DebugTabViewModel DebugTabViewModel { get; }

        public TitleTabViewModel TitleTabViewModel { get; }

        public PrinterTabViewModel PrinterTabViewModel { get; }

        public GeneralTabViewModel GeneralTabViewModel { get; }

        public PdfArchitectTabViewModel PdfArchitectTabViewModel { get; }
    }
}