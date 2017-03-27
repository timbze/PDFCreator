using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.LicenseValidator.Interface;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public class TerminalServerLicenseChecker : ILicenseChecker
    {
        private readonly ILicenseChecker _licenseChecker;

        public TerminalServerLicenseChecker(ILicenseChecker licenseChecker)
        {
            _licenseChecker = licenseChecker;
        }

        public Option<Activation, LicenseError> GetSavedActivation()
        {
            return _licenseChecker.GetSavedActivation();
        }

        public Option<Activation, LicenseError> GetActivation()
        {
            return _licenseChecker.GetSavedActivation();
        }

        public Option<string, LicenseError> GetSavedLicenseKey()
        {
            return _licenseChecker.GetSavedLicenseKey();
        }

        public Option<Activation, LicenseError> ActivateWithKey(string key)
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public Option<Activation, LicenseError> ActivateWithoutSaving(string key)
        {
            return _licenseChecker.ActivateWithoutSaving(key);
        }

        public void SaveActivation(Activation activation)
        {
            
        }
    }
}
