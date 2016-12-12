using System.Linq;
using NSubstitute;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest.Licensing
{
    [TestFixture]
    public class ActivationHelperTerminalServerTest
    {
        private Activation _savedActivationCurrentUser;
        private Activation _savedActivationLocalMachine;
        private ILicenseServerHelper _licenseServerHelper;
        private ILicenseChecker _licenseCheckerCurrentUser;
        private ILicenseChecker _licenseCheckerLocalMachine;

        [SetUp]
        public void SetUp()
        {
            _savedActivationCurrentUser = new Activation();
            _savedActivationCurrentUser.Exists = true; //must be set for LicenseStillValid/ActivationStillValid
            _savedActivationCurrentUser.Key = "CurrentUserKey";
            _licenseCheckerCurrentUser = Substitute.For<ILicenseChecker>();
            _licenseCheckerCurrentUser.GetSavedActivation().Returns(_savedActivationCurrentUser);

            _savedActivationLocalMachine = new Activation();
            _savedActivationLocalMachine.Exists = true; //must be set for LicenseStillValid/ActivationStillValid
            _savedActivationLocalMachine.Key = "LocalMachineKey";
            _licenseCheckerLocalMachine = Substitute.For<ILicenseChecker>();
            _licenseCheckerLocalMachine.GetSavedActivation().Returns(_savedActivationLocalMachine);

            _licenseServerHelper = Substitute.For<ILicenseServerHelper>();
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(_licenseCheckerCurrentUser);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(_licenseCheckerLocalMachine);
        }

        [Test]
        public void RenewActivation_BothLicenseCheckersReceiveNoCalls()
        {
            var activationHelper = new ActivationHelperTerminalServer(Product.PdfCreatorTerminalServer, _licenseServerHelper);
            
            activationHelper.RenewActivation();
            
            Assert.AreEqual(0, _licenseCheckerCurrentUser.ReceivedCalls().Count());    
            Assert.AreEqual(0, _licenseCheckerLocalMachine.ReceivedCalls().Count());
        }
    }
}
