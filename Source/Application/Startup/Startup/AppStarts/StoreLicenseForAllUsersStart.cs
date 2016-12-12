using System;
using System.Security;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class StoreLicenseForAllUsersStart : AppStartBase
    {
        private readonly IActivationHelper _activationHelper;
        private readonly ILicenseServerHelper _licenseServerHelper;

        public StoreLicenseForAllUsersStart(ICheckAllStartupConditions checkAllStartupConditions, IActivationHelper activationHelper, ILicenseServerHelper licenseServerHelper)
            : base(checkAllStartupConditions)
        {
            _activationHelper = activationHelper;
            _licenseServerHelper = licenseServerHelper;
        }

        public override ExitCode Run()
        {
            var activation = _activationHelper.Activation;
            if (activation == null)
                return ExitCode.MissingActivation;

            try
            {
                var licenseCheckerLm = _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine);
                licenseCheckerLm.SaveActivation(activation);
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
