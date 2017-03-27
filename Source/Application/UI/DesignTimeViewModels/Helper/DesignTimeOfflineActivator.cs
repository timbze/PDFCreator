using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    class DesignTimeOfflineActivator : IOfflineActivator
    {
        public string BuildOfflineActivationString(string key)
        {
            return "";
        }

        public Option<Activation, LicenseError> ActivateOfflineActivationString(string encodedOfflineActivationString)
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public Option<Activation, LicenseError> ValidateOfflineActivationString(string encodedOfflineActivationString)
        {
            return Option.None<Activation, LicenseError>(LicenseError.NoActivation);
        }

        public void SaveActivation(Activation activation)
        {
            
        }
    }
}