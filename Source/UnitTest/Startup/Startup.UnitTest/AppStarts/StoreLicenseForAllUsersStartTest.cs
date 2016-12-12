using System;
using System.Security;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UnitTest.Startup.AppStarts
{
    [TestFixture]
    public class StoreLicenseForAllUsersStartTest
    {
        private StoreLicenseForAllUsersStart _storeLicenseForAllUsersStart;
        private ICheckAllStartupConditions _checkAllStartupConditions; 
        private IActivationHelper _activationHelper ;
        private ILicenseServerHelper _licenseServerHelper;
        private ILicenseChecker _licenseChecker;
        private Activation _activation;

        [SetUp]
        public void SetUp()
        {
            _checkAllStartupConditions = Substitute.For<ICheckAllStartupConditions>();
            _activation = new Activation();
            _activationHelper = Substitute.For<IActivationHelper>();
            _activationHelper.Activation.Returns(_activation);
            _licenseServerHelper = Substitute.For<ILicenseServerHelper>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine)
                .Returns(_licenseChecker);

            _storeLicenseForAllUsersStart = new StoreLicenseForAllUsersStart(_checkAllStartupConditions, _activationHelper, _licenseServerHelper);
        }

        [Test]
        public void Run_ActivationIsNull_ReturnsStartupResultMissingActivation()
        {
            _activationHelper.Activation.ReturnsNull();

            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.MissingActivation, result);
        }

        [Test]
        public void Run_ActivationNotNull_LicenseCheckerSavesActivationStartupResultIsOk()
        {
            var result = _storeLicenseForAllUsersStart.Run();

            _licenseChecker.Received().SaveActivation(_activation);
            Assert.AreEqual(ExitCode.Ok, result);
        }

        [Test]
        public void Run_LicenseCheckerSaveActivationThrowsSecurityException_StartUpResultIsNoAccessPrivileges()
        {
            _licenseChecker.When(x => x.SaveActivation(_activation)).Do(x => { throw new SecurityException(); });

            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.NoAccessPrivileges, result);
        }

        [Test]
        public void Run_LicenseCheckerSaveActivationThrowsException_StartUpResultIsUnknown()
        {
            _licenseChecker.When(x => x.SaveActivation(_activation)).Do(x => { throw new Exception(); });

            var result = _storeLicenseForAllUsersStart.Run();

            Assert.AreEqual(ExitCode.Unknown, result);
        }
    }
}
