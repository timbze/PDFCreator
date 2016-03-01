using System;
using NSubstitute;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;

namespace PDFCreator.Shared.Test.Licensing
{
    [TestFixture]
    class EditionFactoryTest
    {
        private IEditionFactory _editionFactory;
        private IVersionHelper _versionHelper;

        private Activation _savedActivation;
        private Product? _licensedProduct = null;

        private Activation ReturnSavedActivation(Product? product)
        {
            _licensedProduct = product;
            return _savedActivation;
        }

        private void SetLicenseStillValid()
        {
            _savedActivation.Result = Result.OK;
            _savedActivation.Exists = true; //must be set for LicenseStillValid
            _savedActivation.LicenseExpires = DateTime.MaxValue;
        }

        private void SetLicenseExpired()
        {
            _savedActivation.Result = Result.OK;
            _savedActivation.Exists = true; //must be set for LicenseStillValid
            _savedActivation.LicenseExpires = DateTime.MinValue;
        }

        private void SetActivationStillValid()
        {
            _savedActivation.Result = Result.OK;
            _savedActivation.Exists = true; //must be set for ActivationStillValid
            _savedActivation.ActivatedTill = DateTime.MaxValue;
        }

        private void SetActivationExpired()
        {
            _savedActivation.Result = Result.OK;
            _savedActivation.Exists = true; //must be set for ActivationStillValid
            _savedActivation.ActivatedTill = DateTime.MinValue;
        }

        [SetUp]
        public void SetUp()
        {
            _versionHelper = Substitute.For<IVersionHelper>();
            _savedActivation = new Activation();
            _savedActivation.Exists = true; //must be set for LicenseStillValid/ActivationStillValid
            _licensedProduct = null;
        }

        [TestCase(LicenseStatus.Valid, Result = true)]
        [TestCase(LicenseStatus.ValidForVersionButLicenseExpired, Result=true)]
        [TestCase(LicenseStatus.LicenseExpired, Result = false)]
        [TestCase(LicenseStatus.ActivationExpired, Result = false)]
        [TestCase(LicenseStatus.Blocked, Result = false)]
        [TestCase(LicenseStatus.Error, Result = false)]
        [TestCase(LicenseStatus.NoLicense, Result = false)]
        [TestCase(LicenseStatus.InvalidLicenseKey, Result = false)]
        [TestCase(LicenseStatus.NoLicenseKey, Result = false)]
        [TestCase(LicenseStatus.NoServerConnection, Result = false)]
        [TestCase(LicenseStatus.NumberOfActivationsExceeded, Result = false)]
        public bool EditionIsValidLicense_DependentOnLincenseStatus(LicenseStatus ls)
        {
            var edition = new Edition();

            edition.LicenseStatus = ls;

            return edition.IsLicenseValid;
        }

        [Test]
        public void InvalidEditionName_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                new EditionFactory("Invalid EditionName", "HasNoInfluence",_versionHelper, ReturnSavedActivation); 
            });
        }

        #region PDFCreator Edition

        [Test]
        public void PDFCreatorEdition_GetSavedActivationWasNotCalled()
        {
            _editionFactory = new EditionFactory("PDFCreator", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(null, _licensedProduct);
        }

        [Test]
        public void PDFCreatorEdition__CurrentActivationIsNull()
        {
            _editionFactory = new EditionFactory("PDFCreator", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(null, _editionFactory.Edition.Activation);
        }

        [Test]
        public void PDFCreatorEdition_CheckGeneralProperties()
        {
            _editionFactory = new EditionFactory("PDFCreator", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            var edition = _editionFactory.Edition;

            Assert.AreEqual("PDFCreator", edition.Name, "Edition name");
            Assert.AreEqual("PDFCreator", edition.UpdateSectionName, "Update section name");
            Assert.AreEqual(Urls.PdfCreatorUpdateInfoUrl, edition.UpdateInfoUrl, "Update info url");  
            Assert.IsFalse(edition.ActivateGpo, "GPO activation");
            Assert.IsFalse(edition.AutomaticUpdate, "Automatic update");
            Assert.IsTrue(edition.ShowPlusHint, "Show plus hint");
            Assert.IsTrue(edition.ShowWelcomeWindow, "Show welcome window");
            Assert.IsTrue(edition.HideLicensing, "Hide licensing");
            Assert.IsFalse(edition.ValidOnTerminalServer, "Valid on Terminal Server");
            Assert.IsFalse(edition.HideAndDisableUpdates, "Hide and disable updates");
            Assert.IsFalse(edition.HideDonateButton, "Hide donate button");
            Assert.IsFalse(edition.HideSocialMediaButtons, "Hide social media buttons");
            Assert.AreSame(_versionHelper, edition.VersionHelper);
        }

        [TestCase(Result.OK)]
        [TestCase(Result.INVALID_LICENSE_KEY)]
        [TestCase(Result.AUTH_FAILED)]
        [TestCase(Result.BLOCKED)]
        [TestCase(Result.LICENSE_EXPIRED)]
        [TestCase(Result.LICENSE_LIMIT_REACHED)]
        [TestCase(Result.NO_LICENSE_KEY)]
        [TestCase(Result.NO_SERVER_CONNECTION)]
        [TestCase(Result.UNKNOWN_VERSION)]
        public void PDFCreatorEdition_CheckLicenseStatus_AlwaysValid(Result activationResult)
        {
            _savedActivation.Result = activationResult;
            _editionFactory = new EditionFactory("PDFCreator", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
      
            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PDFCreatorEdition_LicenseStatusWhenActivationIsNull_ReturnsValid()
        {
            _savedActivation = null;
            _editionFactory = new EditionFactory("PDFCreator", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }
        #endregion

        #region PDFCreator Plus Edition

        [Test]
        public void PDFCreatorPlusEdition_GetSavedActivationWasCalledWithPDFCreatorProduct()
        {
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(Product.PdfCreator, _licensedProduct);
        }

        [Test]
        public void PDFCreatorPlusEdition_CurrentActivationProductIsPDFCreator()
        {
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(_editionFactory.Edition.Activation.Product, Product.PdfCreator);
        }

        [Test]
        public void PDFCreatorPlusEdition_CheckGeneralProperties()
        {
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            var edition = _editionFactory.Edition;

            Assert.AreEqual("PDFCreator Plus", edition.Name, "Edition name");
            Assert.AreEqual("PDFCreatorPlus", edition.UpdateSectionName, "Update section name");
            Assert.AreEqual(Urls.PdfCreatorPlusUpdateInfoUrl, edition.UpdateInfoUrl, "Update info url");
            Assert.IsFalse(edition.ActivateGpo, "GPO activation");
            Assert.IsTrue(edition.AutomaticUpdate, "Automatic update");
            Assert.IsFalse(edition.ShowPlusHint, "Show plus hint");
            Assert.IsTrue(edition.ShowWelcomeWindow, "Show welcome window");
            Assert.IsFalse(edition.HideLicensing, "Hide licensing");
            Assert.IsFalse(edition.ValidOnTerminalServer, "Valid on Terminal Server");
            Assert.IsFalse(edition.HideAndDisableUpdates, "Hide and disable updates");
            Assert.IsTrue(edition.HideDonateButton, "Hide donate button");
            Assert.IsFalse(edition.HideSocialMediaButtons, "Hide social media buttons");
            Assert.AreSame(_versionHelper, _editionFactory.Edition.VersionHelper);
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
            _savedActivation.Result = activationResult;
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            return _editionFactory.Edition.LicenseStatus;
        }

        [Test]
        public void PDFCreatorPlusEdition_LicenseStatusWhenResultIsOkAndLicenseStillValid_ReturnsValid()
        {
            SetLicenseStillValid();
            SetActivationExpired(); //Should have no influence on plus
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PDFCreatorPlusEdition_LicenseStatusWhenResultIsOkAndLicenseExpired_ReturnsValidForVersionButLicenseExpired()
        {
            SetLicenseExpired();
            SetActivationExpired(); //Should have no influence on plus
            _editionFactory = new EditionFactory("PDFCreator Plus", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.ValidForVersionButLicenseExpired, _editionFactory.Edition.LicenseStatus);
        }

        #endregion

        #region PDFCreator Business Edition

        [Test]
        public void PdfCreatorBusiness_GetSavedActivationWasCalledWithPdfCreatorProduct()
        {
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(Product.PdfCreatorBusiness, _licensedProduct);
        }

        [Test]
        public void PDFCreatorBusiness_CurrentActivationProcutIsPDFCreatorBusiness()
        {
            _editionFactory = new EditionFactory("PDFCreator Business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(_editionFactory.Edition.Activation.Product, Product.PdfCreatorBusiness);
        }

        [Test]
        public void PdfCreatorBusiness_CheckGeneralProperties()
        {
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            var edition = _editionFactory.Edition;

            Assert.AreEqual("PDFCreator Business", edition.Name, "Edition name");
            Assert.AreEqual("PDFCreatorBusiness", edition.UpdateSectionName, "Update section name");
            Assert.AreEqual(Urls.PdfCreatorBusinessUpdateInfoUrl, edition.UpdateInfoUrl, "Update info url");
            Assert.IsTrue(edition.ActivateGpo, "GPO activation");
            Assert.IsTrue(edition.AutomaticUpdate, "Automatic update");
            Assert.IsFalse(edition.ShowPlusHint, "Show plus hint");
            Assert.IsFalse(edition.ShowWelcomeWindow, "Show welcome window");
            Assert.IsFalse(edition.HideLicensing, "Hide licensing");
            Assert.IsFalse(edition.ValidOnTerminalServer, "Valid on Terminal Server");
            Assert.IsFalse(edition.HideAndDisableUpdates, "Hide and disable updates");
            Assert.IsTrue(edition.HideDonateButton, "Hide donate button");
            Assert.IsTrue(edition.HideSocialMediaButtons, "Hide social media buttons");
            Assert.AreSame(_versionHelper, edition.VersionHelper);
        }

        [TestCase(Result.INVALID_LICENSE_KEY, Result = LicenseStatus.InvalidLicenseKey)]
        [TestCase(Result.AUTH_FAILED, Result = LicenseStatus.Error)]
        [TestCase(Result.BLOCKED, Result = LicenseStatus.Blocked)]
        [TestCase(Result.LICENSE_EXPIRED, Result = LicenseStatus.VersionNotCoveredByLicense)]
        [TestCase(Result.LICENSE_LIMIT_REACHED, Result = LicenseStatus.NumberOfActivationsExceeded)]
        [TestCase(Result.NO_LICENSE_KEY, Result = LicenseStatus.NoLicenseKey)]
        [TestCase(Result.NO_SERVER_CONNECTION, Result = LicenseStatus.NoServerConnection)]
        [TestCase(Result.UNKNOWN_VERSION, Result = LicenseStatus.Error)]
        public LicenseStatus PdfCreatorBusiness_CheckLicenseStatusForActivationResultsThatAreNotOk(Result activationResult)
        {
            _savedActivation.Result = activationResult;
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            return _editionFactory.Edition.LicenseStatus;
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_LicenseAndActivationStillValid_LicenseStatusReturnsValid()
        {
            SetActivationStillValid();
            SetLicenseStillValid();
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_ActivationStillValid__LicenseExpired_LicenseStatusReturnsLicenseExpired()
        {
            SetActivationStillValid();
            SetLicenseExpired();
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.LicenseExpired, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PdfCreatorBusiness_ActivationResultIsOk_ActivationExpired_LicenseStillValid_LicenseStatusReturnsActivationExpired()
        {
            SetActivationExpired();
            SetLicenseStillValid(); //Should have no influence if activation expired
            _editionFactory = new EditionFactory("pdfcreator business", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.ActivationExpired, _editionFactory.Edition.LicenseStatus);
        }

        #endregion

        #region PDFCreator Terminal Server Edition

        [Test]
        public void PdfCreatorTerminalServer_GetSavedActivationWasCalledWithPdfCreatorTerminalServerProduct()
        {
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(Product.PdfCreatorTerminalServer, _licensedProduct);
        }

        [Test]
        public void PdfCreatorTerminalServer_CurrentActivationProductIsPDFCreatorTerminalServer()
        {
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(_editionFactory.Edition.Activation.Product, Product.PdfCreatorTerminalServer);
        }

        [Test]
        public void PdfCreatorTerminalServer_CheckGeneralProperties()
        {
            _editionFactory = new EditionFactory("PDFCreator Terminal Server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            var edition = _editionFactory.Edition;

            Assert.AreEqual("PDFCreator Terminal Server", edition.Name, "Edition name");
            Assert.AreEqual("PDFCreatorTerminalServer", edition.UpdateSectionName, "Update section name");
            Assert.AreEqual(Urls.PdfCreatorTerminalServerUpdateInfoUrl, edition.UpdateInfoUrl, "Update info url");
            Assert.IsTrue(edition.ActivateGpo, "GPO activation");
            Assert.IsTrue(edition.AutomaticUpdate, "Automatic update");
            Assert.IsFalse(edition.ShowPlusHint, "Show plus hint");
            Assert.IsFalse(edition.ShowWelcomeWindow, "Show welcome window");
            Assert.IsFalse(edition.HideLicensing, "Hide licensing");
            Assert.IsTrue(edition.ValidOnTerminalServer, "Valid on Terminal Server");
            Assert.IsFalse(edition.HideAndDisableUpdates, "Hide and disable updates");
            Assert.IsTrue(edition.HideDonateButton, "Hide donate button");
            Assert.IsTrue(edition.HideSocialMediaButtons, "Hide social media buttons");
            Assert.AreSame(_versionHelper, edition.VersionHelper);
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
            _savedActivation.Result = activationResult;
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            return _editionFactory.Edition.LicenseStatus;
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_LicenseAndActivationStillValid_LicenseStatusReturnsValid()
        {
            SetActivationStillValid();
            SetLicenseStillValid();
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_ActivationStillValid__LicenseExpired_LicenseStatusReturnsLicenseExpired()
        {
            SetActivationStillValid();
            SetLicenseExpired();
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.LicenseExpired, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PdfCreatorTerminalServer_ActivationResultIsOk_ActivationExpired_LicenseStillValid_LicenseStatusReturnsActivationExpired()
        {
            SetActivationExpired();
            SetLicenseStillValid(); //Should have no influence if activation expired
            _editionFactory = new EditionFactory("pdfcreator terminal server", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(LicenseStatus.ActivationExpired, _editionFactory.Edition.LicenseStatus);
        }

        #endregion

        #region PDFCreator Custom Edition

        [Test]
        public void PDFCreatorCustomEdition_GetSavedActivationWasNotCalled()
        {
            _editionFactory = new EditionFactory("PDFCreator Custom", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(null, _licensedProduct);
        }

        [Test]
        public void PDFCreatorCustomEdition_CurrentActivationIsNull()
        {
            _editionFactory = new EditionFactory("PDFCreator Custom", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            Assert.AreEqual(null, _editionFactory.Edition.Activation);
        }

        [Test]
        public void PDFCreatorCustomEdition_CheckGeneralProperties()
        {
            _editionFactory = new EditionFactory("PDFCreator Custom", "HasNoInfluence", _versionHelper, ReturnSavedActivation);
            var edition = _editionFactory.Edition;

            Assert.AreEqual("PDFCreator Custom", edition.Name, "Edition name");
            Assert.AreEqual("", edition.UpdateSectionName, "Update section name");
            Assert.AreEqual("", edition.UpdateInfoUrl, "Update info url");
            Assert.IsTrue(edition.ActivateGpo, "GPO activation");
            Assert.IsFalse(edition.AutomaticUpdate, "Automatic update");
            Assert.IsFalse(edition.ShowPlusHint, "Show plus hint");
            Assert.IsFalse(edition.ShowWelcomeWindow, "Show welcome window");
            Assert.IsTrue(edition.HideLicensing, "Hide licensing");
            Assert.IsTrue(edition.HideAndDisableUpdates, "Hide and disable updates");
            Assert.IsTrue(edition.HideDonateButton, "Hide donate button");
            Assert.IsTrue(edition.HideSocialMediaButtons, "Hide social media buttons");
            Assert.AreSame(_versionHelper, edition.VersionHelper);
        }

        [TestCase("true", Result = true)]
        [TestCase("True", Result = true)]
        [TestCase("false", Result = false)]
        [TestCase("Technically anything else than true", Result = false)]
        public bool PDFCreatorCustomEdition_CheckIsValidOnTerminalServer(string validOnTerminalServer)
        {
            _editionFactory = new EditionFactory("PDFCreator Custom", validOnTerminalServer, _versionHelper, ReturnSavedActivation);
            return _editionFactory.Edition.ValidOnTerminalServer;
        }

        [TestCase(Result.OK)]
        [TestCase(Result.INVALID_LICENSE_KEY)]
        [TestCase(Result.AUTH_FAILED)]
        [TestCase(Result.BLOCKED)]
        [TestCase(Result.LICENSE_EXPIRED)]
        [TestCase(Result.LICENSE_LIMIT_REACHED)]
        [TestCase(Result.NO_LICENSE_KEY)]
        [TestCase(Result.NO_SERVER_CONNECTION)]
        [TestCase(Result.UNKNOWN_VERSION)]
        public void PDFCreatorCustomEdition_CheckLicenseStatus_AlwaysValid(Result activationResult)
        {
            _savedActivation.Result = activationResult;
            _editionFactory = new EditionFactory("PDFCreator Custom", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }

        [Test]
        public void PDFCreatorEditionCustom_LicenseStatusWhenActivationIsNull_ReturnsValid()
        {
            _savedActivation = null;
            _editionFactory = new EditionFactory("PDFCreator Custom", "HasNoInfluence", _versionHelper, ReturnSavedActivation);

            Assert.AreEqual(LicenseStatus.Valid, _editionFactory.Edition.LicenseStatus);
        }
        #endregion
    }
}
