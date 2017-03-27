using System;
using System.Security;
using SystemInterface.Microsoft.Win32;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UnitTest.Startup.AppStarts
{
    [TestFixture]
    public class StoreLicenseForAllUsersStartTest
    {
        private StoreLicenseForAllUsersStart _storeLicenseForAllUsersStart;
        private IRegistry _registry;
        private string _licenseServerString;
        private ICheckAllStartupConditions _checkAllStartupConditions;
        private string _licenseKey;

        private const string RegistryPath = @"SOFTWARE\\test\pdfforge\\PDFCreator";

        [SetUp]
        public void SetUp()
        {
            _checkAllStartupConditions = Substitute.For<ICheckAllStartupConditions>();

            _licenseServerString = "LSA";
            _licenseKey = "MY-LICENSE-KEY";

            _registry = Substitute.For<IRegistry>();

            var pathProvider = Substitute.For<IInstallationPathProvider>();
            pathProvider.ApplicationRegistryPath.Returns(RegistryPath);

            _storeLicenseForAllUsersStart = new StoreLicenseForAllUsersStart(_checkAllStartupConditions, _registry, pathProvider);
            _storeLicenseForAllUsersStart.LicenseServerCode = _licenseServerString;
            _storeLicenseForAllUsersStart.LicenseKey = _licenseKey;
        }

        [Test]
        public void Run_ActivationIsNone_ReturnsStartupResultMissingActivation()
        {
            _storeLicenseForAllUsersStart.LicenseServerCode = null;
            
            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.MissingActivation, result);
        }

        [Test]
        public void Run_ActivationNotNull_LicenseCheckerSavesActivationStartupResultIsOk()
        {
            var result = _storeLicenseForAllUsersStart.Run();

            _registry.Received(1).SetValue("HKEY_LOCAL_MACHINE\\" + RegistryPath, "License", _licenseKey);
            _registry.Received(1).SetValue("HKEY_LOCAL_MACHINE\\" + RegistryPath, "LSA", _licenseServerString);
            Assert.AreEqual(ExitCode.Ok, result);
        }

        [Test]
        public void Run_LicenseCheckerSaveActivationThrowsSecurityException_StartUpResultIsNoAccessPrivileges()
        {
            _registry.When(x => x.SetValue(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())).Do(x => { throw new SecurityException(); });

            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.NoAccessPrivileges, result);
        }

        [Test]
        public void Run_LicenseCheckerSaveActivationThrowsException_StartUpResultIsUnknown()
        {
            _registry.When(x => x.SetValue(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())).Do(x => { throw new Exception(); });

            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.Unknown, result);
        }
    }
}
