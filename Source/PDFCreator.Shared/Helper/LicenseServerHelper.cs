using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public interface ILicenseServerHelper
    {
        ILicenseChecker BuildLicenseChecker(Product product, RegistryHive registryHive);
    }

    public class LicenseServerHelper : ILicenseServerHelper
    {
        public ILicenseChecker BuildLicenseChecker(Product product, RegistryHive registryHive)
        {
            var version = VersionHelper.Instance.FormatWithThreeDigits();
            var config = new Configuration(product, version, @"SOFTWARE\pdfforge\PDFCreator");
            config.RegistryHive = registryHive;

            return new LicenseChecker(config);
        }
    }
}
