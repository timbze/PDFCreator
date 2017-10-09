using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.Translations
{
    public class ProgramTranslation : ITranslatable
    {
        private string ErrorWithLicensedComponent { get; set; } = "There was an error with a component licensed by pdfforge.\nPlease reinstall PDFCreator. (Error {0})";
        private string LicenseInvalid { get; set; } = "Your license for \"{0}\" is invalid or has expired. Please check your license, otherwise PDFCreator will shutdown.\nDo you want to check your license now?";
        private string LicenseInvalidAfterReactivation { get; set; } = "Your license for \"{0}\" has expired.\nPDFCreator will shut down.";
        private string LicenseInvalidGpoHideLicenseTab { get; set; } = "Your license for \"{0}\" has expired.\nPlease contact your administrator.\nPDFCreator will shut down.";
        public string UsePDFCreatorTerminalServer { get; private set; } = "Please use \"PDFCreator Terminal Server\" for use on computers with installed Terminal Services.\n\nPlease visit our website for more information or contact us directly: licensing@pdfforge.org";

        public string GetFormattedLicenseInvalidGpoHideLicenseTab(string editionWithVersionNumber)
        {
            return string.Format(LicenseInvalidGpoHideLicenseTab, editionWithVersionNumber);
        }

        public string GetFormattedLicenseInvalidTranslation(string editionWithVersionNumber)
        {
            return string.Format(LicenseInvalid, editionWithVersionNumber);
        }

        public string GetFormattedLicenseInvalidAfterReactivationTranslation(string applicationName)
        {
            return string.Format(LicenseInvalidAfterReactivation, applicationName);
        }

        public string GetFormattedErrorWithLicensedComponentTranslation(int errorCode)
        {
            return string.Format(ErrorWithLicensedComponent, errorCode);
        }
    }
}
