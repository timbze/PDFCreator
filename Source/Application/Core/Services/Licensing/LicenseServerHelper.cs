using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public interface ILicenseServerHelper
    {
        ILicenseChecker BuildLicenseChecker(RegistryHive registryHive);
    }

    public class LicenseServerHelper : ILicenseServerHelper
    {
        private readonly Product _product;

        public LicenseServerHelper(Product product)
        {
            _product = product;
        }

        public ILicenseChecker BuildLicenseChecker(RegistryHive registryHive)
        {
            var versionHelper = new VersionHelper(new AssemblyHelper());
            var version = versionHelper.FormatWithThreeDigits();
            var config = new Configuration(_product, version, @"SOFTWARE\pdfforge\PDFCreator");
            config.RegistryHive = registryHive;

            return new LicenseChecker(config);
        }
    }

    // TODO Remove this when it is no longer required in ApplicationSettingsViewModel
    public class UnlicensedLicenseServerHelper : ILicenseServerHelper
    {
        public ILicenseChecker BuildLicenseChecker(RegistryHive registryHive)
        {
            return null;
        }
    }
}