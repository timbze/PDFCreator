using System;
using NSubstitute;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest.Licensing
{
    [TestFixture]
    public class ActivationHelperTest
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

        [TestCase(Result.INVALID_LICENSE_KEY, Result = LicenseStatus.InvalidLicenseKey)]
        [TestCase(Result.AUTH_FAILED, Result = LicenseStatus.Error)]
        [TestCase(Result.BLOCKED, Result = LicenseStatus.Blocked)]
        [TestCase(Result.LICENSE_EXPIRED, Result = LicenseStatus.VersionNotCoveredByLicense)]
        [TestCase(Result.LICENSE_LIMIT_REACHED, Result = LicenseStatus.NumberOfActivationsExceeded)]
        [TestCase(Result.NO_LICENSE_KEY, Result = LicenseStatus.NoLicenseKey)]
        [TestCase(Result.NO_SERVER_CONNECTION, Result = LicenseStatus.NoServerConnection)]
        [TestCase(Result.UNKNOWN_VERSION, Result = LicenseStatus.Error)]
        public LicenseStatus PDFCreatorEdition_CheckLicenseStatusForActivationResultsThatAreNotOk(Result activationResult)
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = activationResult;
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.LoadActivation();

            Assert.IsFalse(activationHelper.IsLicenseValid);

            return activationHelper.LicenseStatus;
        }

        [TestCase(Result.INVALID_LICENSE_KEY, Result = LicenseStatus.InvalidLicenseKey)]
        [TestCase(Result.AUTH_FAILED, Result = LicenseStatus.Error)]
        [TestCase(Result.BLOCKED, Result = LicenseStatus.Blocked)]
        [TestCase(Result.LICENSE_EXPIRED, Result = LicenseStatus.VersionNotCoveredByLicense)]
        [TestCase(Result.LICENSE_LIMIT_REACHED, Result = LicenseStatus.NumberOfActivationsExceeded)]
        [TestCase(Result.NO_LICENSE_KEY, Result = LicenseStatus.NoLicenseKey)]
        [TestCase(Result.NO_SERVER_CONNECTION, Result = LicenseStatus.NoServerConnection)]
        [TestCase(Result.UNKNOWN_VERSION, Result = LicenseStatus.Error)]
        public LicenseStatus PdfCreatorTerminalServer_CheckLicenseStatusForActivationResultsThatAreNotOk(Result activationResult)
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = activationResult;
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            Assert.IsFalse(activationHelper.IsLicenseValid);

            return activationHelper.LicenseStatus;
        }

        [Test]
        public void PDFCreatorCustomEdition_CheckLicenseStatus_AlwaysValid()
        {
            var activationHelper = new UnlicensedActivationHelper();

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.Valid, activationHelper.LicenseStatus);
        }

        private ILicenseServerHelper BuildLicenseServerHelperWithActivationTimes(DateTime hkcuActivationTime, DateTime hklmActivationTime)
        {
            var hkcuActivation = new Activation();
            var hkcuLicenseChecker = Substitute.For<ILicenseChecker>();
            hkcuLicenseChecker.GetSavedActivation().Returns(hkcuActivation);
            var hklmActivation = new Activation();
            var hklmLicenseChecker = Substitute.For<ILicenseChecker>();
            hklmLicenseChecker.GetSavedActivation().Returns(hklmActivation);

            var licenseServerHelper = Substitute.For<ILicenseServerHelper>();
            licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(hkcuLicenseChecker);
            licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(hklmLicenseChecker);

            hkcuActivation.TimeOfActivation = hkcuActivationTime;
            hklmActivation.TimeOfActivation = hklmActivationTime;

            return licenseServerHelper;
        }

        [Test]
        public void GetSavedActivation_ActivationInHKCUisNewer_ReturnActivationFromHKCU()
        {
            var hkcuActivationTime = DateTime.Now.AddMinutes(1);
            var hklmActivationTime = DateTime.Now;
            var licenseServerHelper = BuildLicenseServerHelperWithActivationTimes(hkcuActivationTime, hklmActivationTime);

            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness, licenseServerHelper);

            activationHelper.LoadActivation();

            Assert.AreEqual(hkcuActivationTime, activationHelper.Activation.TimeOfActivation);
        }

        [Test]
        public void GetSavedActivation_ActivationInHKLMAndHKCUhaveDateMinActivationTimeAndHKCUHasKey_ReturnActivationFromHKCU()
        {
            var hkcuActivation = new Activation();
            var hkcuLicenseChecker = Substitute.For<ILicenseChecker>();
            hkcuLicenseChecker.GetSavedActivation().Returns(hkcuActivation);
            var hklmActivation = new Activation();
            var hklmLicenseChecker = Substitute.For<ILicenseChecker>();
            hklmLicenseChecker.GetSavedActivation().Returns(hklmActivation);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(hkcuLicenseChecker);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(hklmLicenseChecker);

            hkcuActivation.TimeOfActivation = DateTime.MinValue;
            hkcuActivation.Key = "HAS KEY at least the string is not empty";
            hklmActivation.TimeOfActivation = DateTime.MinValue;

            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(hkcuActivation, activationHelper.Activation);
        }

        [Test]
        public void GetSavedActivation_ActivationInHKLMAndHKCUhaveDateMinActivationTimeAndHKCUHasNoKey_ReturnActivationFromHKLM()
        {
            var hkcuActivation = new Activation();
            var hkcuLicenseChecker = Substitute.For<ILicenseChecker>();
            hkcuLicenseChecker.GetSavedActivation().Returns(hkcuActivation);
            var hklmActivation = new Activation();
            var hklmLicenseChecker = Substitute.For<ILicenseChecker>();
            hklmLicenseChecker.GetSavedActivation().Returns(hklmActivation);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(hkcuLicenseChecker);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(hklmLicenseChecker);

            hkcuActivation.TimeOfActivation = DateTime.MinValue;
            hkcuActivation.Key = "";
            hklmActivation.TimeOfActivation = DateTime.MinValue;

            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(hklmActivation, activationHelper.Activation);
        }

        [Test]
        public void GetSavedActivation_ActivationInHKLMAndHKCUhaveSameActivationTime_ReturnActivationFromHKCU()
        {
            var hkcuActivation = new Activation();
            var hkcuLicenseChecker = Substitute.For<ILicenseChecker>();
            hkcuLicenseChecker.GetSavedActivation().Returns(hkcuActivation);
            var hklmActivation = new Activation();
            var hklmLicenseChecker = Substitute.For<ILicenseChecker>();
            hklmLicenseChecker.GetSavedActivation().Returns(hklmActivation);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(hkcuLicenseChecker);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(hklmLicenseChecker);

            var now = DateTime.Now;
            hkcuActivation.TimeOfActivation = now;
            hklmActivation.TimeOfActivation = now;

            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(hkcuActivation, activationHelper.Activation);
        }

        [Test]
        public void GetSavedActivation_ActivationInHKLMisNewer_ReturnActivationFromHKLM()
        {
            var hkcuActivation = new Activation();
            var hkcuLicenseChecker = Substitute.For<ILicenseChecker>();
            hkcuLicenseChecker.GetSavedActivation().Returns(hkcuActivation);
            var hklmActivation = new Activation();
            var hklmLicenseChecker = Substitute.For<ILicenseChecker>();
            hklmLicenseChecker.GetSavedActivation().Returns(hklmActivation);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser).Returns(hkcuLicenseChecker);
            _licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine).Returns(hklmLicenseChecker);

            var now = DateTime.Now;
            hkcuActivation.TimeOfActivation = now;
            hklmActivation.TimeOfActivation = now.AddMinutes(1);

            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(hklmActivation, activationHelper.Activation);
        }

        [Test]
        public void GetOfflineActivationString_CallsLicenseCheckerHkcu()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.GetOfflineActivationString("key");

            _licenseCheckerCurrentUser.Received().GetOfflineActivationString("key");
        }

        [Test]
        public void ActivateWithoutSavingActivation_CallsLicenseCheckerHkcu()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.ActivateWithoutSavingActivation("key");

            _licenseCheckerCurrentUser.Received().ActivateWithoutSavingActivation("key");
        }

        [Test]
        public void ActivateOfflineActivationStringFromLicenseServer_CallsLicenseCheckerHkcu()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.ActivateOfflineActivationStringFromLicenseServer("lsa");

            _licenseCheckerCurrentUser.Received().ActivateOfflineActivationStringFromLicenseServer("lsa");
        }

        [Test]
        public void PDFCreatorPlusEdition_CurrentActivationProductIsPDFCreator()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.LoadActivation();

            Assert.AreEqual(Product.PdfCreator, activationHelper.Activation.Product);
        }

        [Test]
        public void PDFCreatorPlusEdition_LicenseServerHelperBuildLicenseChecker_WasCalledForHKCUandHKLM()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.LoadActivation();

            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.CurrentUser);
            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.LocalMachine);
        }

        [Test]
        public void PDFCreatorPlusEdition_LicenseStatusWhenResultIsOkAndLicenseExpired_ReturnsValidForVersionButLicenseExpired()
        {
            SetLicenseExpired();
            SetActivationExpired(); //Should have no influence on plus
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.ValidForVersionButLicenseExpired, activationHelper.LicenseStatus);
        }

        [Test]
        public void PDFCreatorPlusEdition_LicenseStatusWhenResultIsOkAndLicenseStillValid_ReturnsValid()
        {
            SetLicenseStillValid();
            SetActivationExpired(); //Should have no influence on plus
            var activationHelper = BuildActivationHelper(Product.PdfCreator);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.Valid, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_ActivationExpired_LicenseStillValid_LicenseStatusReturnsActivationExpired()
        {
            SetActivationExpired();
            SetLicenseStillValid(); //Should have no influence if activation expired
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.ActivationExpired, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_ActivationStillValid__LicenseExpired_LicenseStatusReturnsLicenseExpired()
        {
            SetActivationStillValid();
            SetLicenseExpired();
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.LicenseExpired, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_LicenseAndActivationStillValid_LicenseStatusReturnsValid()
        {
            SetActivationStillValid();
            SetLicenseStillValid();
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.Valid, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_ActivationExpired_LicenseStillValid_LicenseStatusReturnsActivationExpired()
        {
            SetActivationExpired();
            SetLicenseStillValid(); //Should have no influence if activation expired
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.ActivationExpired, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_ActivationStillValid__LicenseExpired_LicenseStatusReturnsLicenseExpired()
        {
            SetActivationStillValid();
            SetLicenseExpired();
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.LicenseExpired, activationHelper.LicenseStatus);
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_LicenseAndActivationStillValid_LicenseStatusReturnsValid()
        {
            SetActivationStillValid();
            SetLicenseStillValid();
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(LicenseStatus.Valid, activationHelper.LicenseStatus);
        }

        [Test]
        public void PDFCreatorBusiness_CurrentActivationProcutIsPDFCreatorBusiness()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            Assert.AreEqual(Product.PdfCreatorBusiness, activationHelper.Activation.Product);
        }

        [Test]
        public void PdfCreatorBusiness_LicenseServerHelperBuildLicenseChecker_WasCalledForHKCUandHKLM()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.CurrentUser);
            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.LocalMachine);
        }

        [Test]
        public void PdfCreatorTerminalServer_CurrentActivationProductIsPDFCreatorTerminalServer()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            Assert.AreEqual(activationHelper.Activation.Product, Product.PdfCreatorTerminalServer);
        }

        [Test]
        public void PdfCreatorTerminalServer_LicenseServerHelperBuildLicenseChecker_WasCalledForHKCUandHKLM()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorTerminalServer);

            activationHelper.LoadActivation();

            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.CurrentUser);
            _licenseServerHelper.Received().BuildLicenseChecker(RegistryHive.LocalMachine);
        }

        [Test]
        public void WhenSavedActivationIsNotValidBase64_HandlesFormatExceptionProperly()
        {
            var licenseCheckerWithException = Substitute.For<ILicenseChecker>();
            licenseCheckerWithException
                .When(x => x.GetSavedActivation())
                .Do(x => { throw new FormatException(); });
            var licenseChecker = Substitute.For<ILicenseChecker>();
            licenseChecker.GetSavedActivation().Returns(_savedActivationCurrentUser);

            _licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser)
                .Returns(licenseCheckerWithException);
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.LoadActivation();

            var newActivation = new Activation();
            newActivation.Product = Product.PdfCreatorBusiness;
            
        }

        [Test]
        public void SaveActivation_CallsLicenseCheckerCurrentUserSaveActivation()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);

            activationHelper.SaveActivation();

            _licenseCheckerCurrentUser.Received().SaveActivation(activationHelper.Activation);
        }

        [Test]
        public void RenewActivation_CallsLicenseCheckerCurrentUserSaveActivation()
        {
            var activationHelper = BuildActivationHelper(Product.PdfCreatorBusiness);
            activationHelper.Activation = new Activation();
            activationHelper.Activation.Key = "Key";

            activationHelper.RenewActivation();

            _licenseCheckerCurrentUser.Received().ActivateWithKey("Key");
        }

        private ActivationHelper BuildActivationHelper(Product product, ILicenseServerHelper licenseServerHelper = null)
        {
            if (licenseServerHelper == null)
                licenseServerHelper = _licenseServerHelper;

            var activationHelper = new ActivationHelper(product, licenseServerHelper);

            if (product == Product.PdfCreator)
            {
                activationHelper.AcceptExpiredActivation = true;
                activationHelper.AcceptExpiredLicense = true;
            }

            return activationHelper;
        }


        private void SetLicenseStillValid()
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = Result.OK;
            _savedActivationCurrentUser.Exists = true; //must be set for LicenseStillValid
            _savedActivationCurrentUser.LicenseExpires = DateTime.MaxValue;
        }

        private void SetLicenseExpired()
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = Result.OK;
            _savedActivationCurrentUser.Exists = true; //must be set for LicenseStillValid
            _savedActivationCurrentUser.LicenseExpires = DateTime.MinValue;
        }

        private void SetActivationStillValid()
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = Result.OK;
            _savedActivationCurrentUser.Exists = true; //must be set for ActivationStillValid
            _savedActivationCurrentUser.ActivatedTill = DateTime.MaxValue;
        }

        private void SetActivationExpired()
        {
            //set result in current user because it gets preferred
            _savedActivationCurrentUser.Result = Result.OK;
            _savedActivationCurrentUser.Exists = true; //must be set for ActivationStillValid
            _savedActivationCurrentUser.ActivatedTill = DateTime.MinValue;
        }
    }
}
