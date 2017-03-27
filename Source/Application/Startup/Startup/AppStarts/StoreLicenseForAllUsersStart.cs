using System;
using System.Security;
using SystemInterface.Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class StoreLicenseForAllUsersStart : AppStartBase
    {
        private readonly IRegistry _registry;
        private string _registryPath;

        public string LicenseServerCode { get; set; }
        public string LicenseKey { get; set; }

        public StoreLicenseForAllUsersStart(ICheckAllStartupConditions checkAllStartupConditions, IRegistry registry, IInstallationPathProvider installationPathProvider)
            : base(checkAllStartupConditions)
        {
            _registry = registry;
            _registryPath = installationPathProvider.ApplicationRegistryPath;
            SkipStartupConditionCheck = true;
        }

        public override ExitCode Run()
        {
            if (string.IsNullOrWhiteSpace(LicenseServerCode))
                return ExitCode.MissingActivation;

            var regPath = "HKEY_LOCAL_MACHINE\\" + _registryPath;

            try
            {
                _registry.SetValue(regPath, "License", LicenseKey);
                _registry.SetValue(regPath, "LSA", LicenseServerCode);
            }
            catch (SecurityException)
            {
                return ExitCode.NoAccessPrivileges;
            }
            catch (Exception)
            {
                return ExitCode.Unknown;
            }
            
            return ExitCode.Ok;
        }
    }
}
