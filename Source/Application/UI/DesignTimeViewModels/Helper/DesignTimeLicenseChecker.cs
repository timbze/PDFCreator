using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    class DesignTimeLicenseChecker : ILicenseChecker
    {
        public Option<Activation, LicenseError> GetSavedActivation()
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public Option<Activation, LicenseError> GetActivation()
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public Option<string, LicenseError> GetSavedLicenseKey()
        {
            return Option.None<string, LicenseError>(LicenseError.NoActivation);
        }

        public Option<Activation, LicenseError> ActivateWithKey(string key)
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public Option<Activation, LicenseError> ActivateWithoutSaving(string key)
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public void SaveActivation(Activation activation)
        {
            
        }
    }
}
