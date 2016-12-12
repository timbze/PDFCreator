using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public class ActivationHelperTerminalServer : ActivationHelper
    {
        public ActivationHelperTerminalServer(Product licenseServerProduct, ILicenseServerHelper licenseServerHelper) 
            : base(licenseServerProduct, licenseServerHelper)
        {   }

        /// <summary>
        /// No functionality for Terminal Server
        /// </summary>
        public override void RenewActivation()
        {   }
    }
}
