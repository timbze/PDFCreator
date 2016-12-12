namespace pdfforge.PDFCreator.Conversion.Settings.GroupPolicies
{
    public class GpoSettingsDefaults : IGpoSettings
    {
        public bool DisableApplicationSettings => false;
        public bool DisableDebugTab => false;
        public bool DisablePrinterTab => false;
        public bool DisableProfileManagement => false;
        public bool DisableTitleTab => false;
        public bool HideLicenseTab => false;
        public bool HidePdfArchitectInfo => false;
        public string Language => null;
        public string UpdateInterval => null;
    }
}