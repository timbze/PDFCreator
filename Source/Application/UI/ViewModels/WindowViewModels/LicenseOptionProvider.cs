namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class LicenseOptionProvider
    {
        public LicenseOptionProvider(bool hideLicenseOptions)
        {
            HideLicenseOptions = hideLicenseOptions;
        }

        public bool HideLicenseOptions { get; }
    }
}