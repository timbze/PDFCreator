using Microsoft.Win32;

namespace pdfforge.PDFCreator.Helper
{
    internal class LicenseHelper
    {
        public static bool HasPlusLicense
        {
            get { return !string.IsNullOrWhiteSpace(PlusLicenseKey); }
        }

        private static string _plusLicenseKey;
        public static string PlusLicenseKey
        {
            get
            {
                if (_plusLicenseKey == null)
                    _plusLicenseKey = GetLicenseKey();

                return _plusLicenseKey;
            }
        }

        private static string GetLicenseKey()
        {
            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            var key = baseKey.OpenSubKey(@"Software\pdfforge\PDFCreator");

            if (key == null)
                return null;

            var licenseKey = key.GetValue("License") as string;

            return licenseKey;
        }
    }
}
