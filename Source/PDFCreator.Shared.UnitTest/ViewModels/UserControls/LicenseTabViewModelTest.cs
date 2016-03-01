using System;
using System.Collections.Generic;
using System.Globalization;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;

namespace PDFCreator.Shared.Test.ViewModels.UserControls
{
    [TestFixture]
    public class LicenseTabViewModelTest
    {
        private ILicenseChecker _licenseChecker;
        private ILicenseServerHelper _licenseServerHelper;
        private string _expectedLicenseKey;
        private IEditionFactory _editionFactory;
        private Edition _edition;
        private Edition _reloadedEdition;

        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(150);

        private Activation _licenseCheckerActivation;

        private const string ValidLicenseKey = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";

        [SetUp]
        public void Setup()
        {
            _licenseCheckerActivation = null;
            _expectedLicenseKey = null;

            _licenseServerHelper = Substitute.For<ILicenseServerHelper>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseServerHelper.BuildLicenseChecker(Arg.Any<Product>(), Arg.Any<RegistryHive>()).Returns(_licenseChecker);
            _licenseChecker.ActivateWithoutSavingActivation(Arg.Any<string>()).Returns(key => _licenseCheckerActivation);
        }

        [Test]
        public void Initialize_SetsEdition()
        {
            var editionHelper = BuildValidEditionHelper();

            var viewModel = BuildViewModel(editionHelper);

            Assert.AreSame(editionHelper.Edition, viewModel.Edition);
        }

        [Test]
        public void LicenseExpiryDateString_WithActivationNull_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithMinExpiryDate_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.LicenseExpires = DateTime.MinValue;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLifetimeLicense_ReturnsNever()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.LicenseExpires = new DateTime(2038, 01, 01);

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("Never", viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLimitedLicense_ReturnsCorrectDate()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.LicenseExpires = DateTime.Now;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual(DateTime.Today, DateTime.Parse(viewModel.LicenseExpiryDate));
        }

        [Test]
        public void LicenseKeyString_WithNullStringKey_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.Key = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("", viewModel.LicenseKey);
        }

        [Test]
        public void LastActivation_WithNullActivation_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("", viewModel.LastActivationTime);
        }
            
        [Test]
        public void LastActivation_WithTimeOfActivationOnDateMinValue_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.TimeOfActivation = DateTime.MinValue;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("", viewModel.LastActivationTime);
        }
        
        [Test]
        public void LastActivation_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            var date = DateTime.Now;
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.TimeOfActivation = date;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.LastActivationTime);
        }
        
        [Test]
        public void ActivationValidTill_WithNullActivation_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }
        
        [Test]
        public void ActivationValidTill_WithActivationTillDateMinValue_ReturnsEmptyString()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.ActivatedTill = DateTime.MinValue;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            var date = DateTime.Now;
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.ActivatedTill = date;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.ActivationValidTill);
        }

        [Test]
        public void LicenseStatus_ReturnsStatusFromEdition()
        {
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            viewModel.Edition.LicenseStatus = LicenseStatus.Blocked;

            Assert.AreEqual(LicenseStatus.Blocked, viewModel.LicenseStatus);
            Assert.AreEqual(EnumToStringValueHelper.GetStringValue(LicenseStatus.Blocked), viewModel.LicenseStatusText);
        }

        [Test]
        public void LicenseKeyString_WithExpectedLength_IsFormattedWithDashes()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";

            var viewModel = BuildViewModel(_editionFactory);

            Assert.AreEqual("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-FFFFF", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithUnexpectedLength_IsFormattedWithDashes()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEF";

            var viewModel = BuildViewModel(_editionFactory);
            
            Assert.AreEqual("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-F", viewModel.LicenseKey);
        }
        
        [Test]
        public void RenewActivationCommand_WithNoEdition_IsNotExecutable()
        {
            _editionFactory = BuildValidEditionHelper();
            _editionFactory.Edition.ReturnsNull();

            var viewModel = BuildViewModel(_editionFactory);

            Assert.IsFalse(viewModel.RenewActivationCommand.CanExecute(null));
        }
        
        [Test]
        public void RenewActivationCommand_WithNoActivation_IsNotExecutable()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.IsFalse(viewModel.RenewActivationCommand.CanExecute(null));
        }
        
        [Test]
        public void RenewActivationCommand_WithValidActivation_IsExecutable()
        {
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            Assert.IsTrue(viewModel.RenewActivationCommand.CanExecute(null));
        }
        
        [Test]
        public void RenewActivationCommand_WhenExecuted_CallsLicenseChecker()
        {
            _editionFactory = BuildValidEditionHelper();
            _licenseCheckerActivation = _edition.Activation;

            var viewModel = BuildViewModel(_editionFactory);

            viewModel.RenewActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.Received().ActivateWithoutSavingActivation(ValidLicenseKey);
        }
        
        [Test]
        public void RenewActivationCommand_WithoutoutActivation_IsNotExecutable()
        {
            _editionFactory = BuildValidEditionHelper();
            _edition.Activation = null;

            var viewModel = BuildViewModel(_editionFactory);

            Assert.IsFalse(viewModel.EnterLicenseKeyCommand.CanExecute(null));
        }
        
        [Test]
        public void RenewActivationCommand_WithProduct_IsExecutable()
        {
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            Assert.IsTrue(viewModel.EnterLicenseKeyCommand.CanExecute(null));
        }
        
        [Test]
        public void RenewActivationCommand_WhileExecuting_NotExecutable()
        {
            bool wasExecutable = true;
            _editionFactory = BuildValidEditionHelper();
            _licenseCheckerActivation = _edition.Activation;

            var viewModel = BuildViewModel(_editionFactory);
            viewModel.ActivationResponse += (sender, args) => wasExecutable = viewModel.RenewActivationCommand.IsExecutable;

            viewModel.RenewActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsFalse(wasExecutable);
        }
        
        [Test]
        public void EnterLicenseKeyCommand_WhenKeyIsNull_DoesNotCallLicenseChecker()
        {
            _expectedLicenseKey = null;
            _editionFactory = BuildValidEditionHelper();
            _licenseCheckerActivation = _edition.Activation;

            var viewModel = BuildViewModel(_editionFactory);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            _licenseChecker.DidNotReceiveWithAnyArgs().ActivateWithKey(_expectedLicenseKey);
        }
        
        [Test]
        public void EnterLicenseKeyCommand_WhenExecuted_CallsLicenseChecker()
        {
            _expectedLicenseKey = "ABCDEF";
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.Received().ActivateWithoutSavingActivation(_expectedLicenseKey);
        }
        
        [Test]
        public void EnterLicenseKeyCommand_WhenExecutedSuccessfully_CallsPropertyChangedForEdition()
        {
            _expectedLicenseKey = "ABCDEF";
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.Contains(nameof(Edition), propertyChangedEvents);
        }
        
        [Test]
        public void EnterLicenseKeyCommand_WhenExecutedSuccessfully_RaisesActivationResponse()
        {
            _expectedLicenseKey = "ABCDEF";
            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            bool eventWasRaised = false;
            viewModel.ActivationResponse += (sender, args) => eventWasRaised = true;

            viewModel.EnterLicenseKeyCommand.Execute(null);

            var waitOneSucceeded = viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsTrue(waitOneSucceeded);
            Assert.IsTrue(eventWasRaised);
        }
        
        [Test]
        public void EnterLicenseKeyCommand_WhenExecuted_RaisesPropertyChanged()
        {
            _expectedLicenseKey = "ABCDEF";

            _editionFactory = BuildValidEditionHelper();

            var viewModel = BuildViewModel(_editionFactory);

            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            Assert.Contains(nameof(viewModel.Edition), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.IsCheckingLicense), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseKey), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatus), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusText), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseExpiryDate), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LastActivationTime), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.ActivationValidTill), propertyChangedEvents);
        }

        [Test]
        public void EnterLicenseKeyCommand_WhileExecuting_NotExecutable()
        {
            _expectedLicenseKey = "ABCDEF";
            bool wasExecutable = true;
            _editionFactory = BuildValidEditionHelper();
            var viewModel = BuildViewModel(_editionFactory);
            viewModel.ActivationResponse += (sender, args) => wasExecutable = viewModel.EnterLicenseKeyCommand.IsExecutable;

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsFalse(wasExecutable);
        }

        [Test]
        public void EnterLicenseKeyCommand_CurrentEditionIsValid_LicenseCheckerActivationIsValid_SaveNewActivationAndUpdateEdition()
        {
            _expectedLicenseKey = "not empty";
            _licenseCheckerActivation = CreateValidActivation();
            _editionFactory = BuildValidEditionHelper();
            var validEdition = CreateValidEdition();
            _editionFactory.DetermineEdition(_licenseCheckerActivation).Returns(validEdition);
            _editionFactory.ReloadEdition().Returns(validEdition);
            var viewModel = BuildViewModel(_editionFactory);
            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _editionFactory.Received().ReloadEdition();
            _licenseChecker.Received().SaveActivation(validEdition.Activation);
            Assert.Contains(nameof(viewModel.Edition), propertyChangedEvents);
            Assert.AreSame(validEdition, viewModel.Edition);
        }

        [Test]
        public void EnterLicenseKeyCommand_CurrentEditionIsValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewActivationAndDoNotUpdateEdition()
        {
            _expectedLicenseKey = "not empty";
            _licenseCheckerActivation = CreateValidActivation();
            _editionFactory = BuildValidEditionHelper();
            var invalidEdition = CreateValidEdition();
            invalidEdition.LicenseStatus = LicenseStatus.Error; //make received Edition invalid
            _editionFactory.DetermineEdition(_licenseCheckerActivation).Returns(invalidEdition);
            _editionFactory.ReloadEdition().Returns(invalidEdition);
            var viewModel = BuildViewModel(_editionFactory);
            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _editionFactory.DidNotReceive().ReloadEdition();
            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());
            Assert.Contains(nameof(viewModel.Edition), propertyChangedEvents);
            Assert.AreSame(_editionFactory.Edition, viewModel.Edition); //Keep old edition 
        }

        [Test]
        public void EnterLicenseKeyCommand_CurrentEditionIsNotValid_LicenseCheckerActivationIsValid_SaveNewActivationAndUpdateEdition()
        {
            _expectedLicenseKey = "not empty";
            _licenseCheckerActivation = CreateValidActivation();
            _editionFactory = BuildValidEditionHelper();
            _editionFactory.Edition.LicenseStatus = LicenseStatus.Error;
            var validEdition = CreateValidEdition();
            _editionFactory.DetermineEdition(_licenseCheckerActivation).Returns(validEdition);
            _editionFactory.ReloadEdition().Returns(validEdition);
            var viewModel = BuildViewModel(_editionFactory);
            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _editionFactory.Received().ReloadEdition();
            _licenseChecker.Received().SaveActivation(validEdition.Activation);
            Assert.Contains(nameof(viewModel.Edition), propertyChangedEvents);
            Assert.AreSame(validEdition, viewModel.Edition);
        }

        [Test]
        public void EnterLicenseKeyCommand_CurrentEditionIsNotValid_LicenseCheckerActivationIsNotValid_UpdateEditionWithGivenKeyDoNotSaveNewEdition()
        {
            _expectedLicenseKey = "given-key";
            _licenseCheckerActivation = CreateValidActivation();
            _editionFactory = BuildValidEditionHelper();
            _editionFactory.Edition.LicenseStatus = LicenseStatus.Error;
            var invalidEdition = CreateValidEdition();
            invalidEdition.LicenseStatus = LicenseStatus.Error; //make received Edition invalid
            _editionFactory.DetermineEdition(_licenseCheckerActivation).Returns(invalidEdition);
            _editionFactory.ReloadEdition().Returns(invalidEdition);
            var viewModel = BuildViewModel(_editionFactory);
            List<string> propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.EnterLicenseKeyCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _editionFactory.DidNotReceive().ReloadEdition();
            _licenseChecker.DidNotReceive().SaveActivation(null);
            Assert.Contains(nameof(viewModel.Edition), propertyChangedEvents);
            Assert.AreSame(invalidEdition, viewModel.Edition);
            Assert.AreEqual(_expectedLicenseKey.Replace("-", ""), viewModel.Edition.Activation.Key, "Given key not set in updated license");
        }

        [TestCase("")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAA")]
        [TestCase("!!!!!-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        public void IsValidNewLicenseKey_WithInvalidKey_IsFalse(string testKey)
        {
            _editionFactory = BuildValidEditionHelper();
            var viewModel = BuildViewModel(_editionFactory);

            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsFalse(validationResult.IsValid, validationResult.Message);
        }

        [TestCase("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345")]
        [TestCase("ZZZZZ-YYYYY-DEFGH-DDDDD-12345-67890")]
        public void IsValidNewLicenseKey_WithValidKey_IsTrue(string testKey)
        {
            _editionFactory = BuildValidEditionHelper();
            var viewModel = BuildViewModel(_editionFactory);

            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsTrue(validationResult.IsValid, validationResult.Message);
        }
        
        private LicenseTabViewModel BuildViewModel(IEditionFactory editionFactory, Func<string> queryKeyFunc = null)
        {
            if (queryKeyFunc == null)
                queryKeyFunc = () => _expectedLicenseKey;

            return new LicenseTabViewModel(editionFactory, _licenseServerHelper, queryKeyFunc, new BasicTranslator("None", Data.CreateDataStorage()));
        }

        private IEditionFactory BuildValidEditionHelper()
        {
            _edition = CreateValidEdition();
            var editionHelper = Substitute.For<IEditionFactory>();
            editionHelper.Edition.Returns(_edition);
            _reloadedEdition = _edition;
            editionHelper.ReloadEdition().Returns(_reloadedEdition);
            editionHelper.DetermineEdition(new Activation()).ReturnsForAnyArgs(_edition); 

            return editionHelper;
        }

        private Edition CreateValidEdition()
        {
            var edition = new Edition();
            edition.LicenseStatus = LicenseStatus.Valid;
            edition.Activation = CreateValidActivation();

            return edition;
        }

        private Activation CreateValidActivation()
        {
            var activation = new Activation();
            activation.Product = Product.PdfCreator;
            activation.Key = ValidLicenseKey;
            activation.Exists = true; // must be set for IsLicenseStillValid
            activation.LicenseExpires = DateTime.Today.AddDays(5);
            activation.ActivatedTill = DateTime.Today.AddDays(5);
            activation.Result = Result.OK;

            return activation;
        }
    }
}
