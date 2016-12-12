using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public interface IActivationHelper
    {
        Activation Activation { get; set; }
        bool IsLicenseValid { get; }
        LicenseStatus LicenseStatus { get; }
        void LoadActivation();
        void SaveActivation();
        void RenewActivation();
        string GetOfflineActivationString(string licenseKey);
        Activation ActivateWithoutSavingActivation(string licenseKey);
        Activation ActivateOfflineActivationStringFromLicenseServer(string lsa);
    }
}