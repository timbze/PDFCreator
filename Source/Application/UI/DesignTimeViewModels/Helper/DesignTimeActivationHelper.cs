using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeActivationHelper : IActivationHelper
    {
        public Activation Activation { get; set; }
        public bool IsLicenseValid { get; }
        public LicenseStatus LicenseStatus { get; }
        public void LoadActivation()
        {   }
        public void SaveActivation()
        {   }
        public void RenewActivation()
        {   }
        public string GetOfflineActivationString(string licenseKey)
        {   return "";   }
        public Activation ActivateWithoutSavingActivation(string licenseKey)
        { return null; }
        public Activation ActivateOfflineActivationStringFromLicenseServer(string lsa)
        { return null; }
    }
}