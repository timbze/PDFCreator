using System;
using System.Collections.Generic;
using System.Globalization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    [TestFixture]
    public class LicenseTabViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _licenseCheckerActivation = null;
            _expectedLicenseKey = null;

            _process = Substitute.For<IProcessStarter>();
            _activationHelper = Substitute.For<IActivationHelper>();
            _activationHelper.Activation.Returns(_licenseCheckerActivation);
            _activationHelper.ActivateWithoutSavingActivation(Arg.Any<string>()).Returns(key => _licenseCheckerActivation);
            _translator = BuildTranslator();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            
            _dispatcher = Substitute.For<IDispatcher>();
            _dispatcher.WhenForAnyArgs(x => x.Invoke(Arg.Any<Action>())).Do(x => ((Action) x[0]).Invoke());
            _dispatcher.WhenForAnyArgs(x => x.BeginInvoke(Arg.Any<Action>())).Do(x => ((Action) x[0]).Invoke());
        }

        private IProcessStarter _process;
        private ITranslator _translator;
        private IInteractionInvoker _interactionInvoker;

        private string _expectedLicenseKey;

        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(150);

        private Activation _licenseCheckerActivation;

        private const string ValidLicenseKey = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";
        private const string ValidLicenseKey_Normalized = "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-FFFFF";

        private IDispatcher _dispatcher;
        private IActivationHelper _activationHelper;

        private const string ActivationSuccessfulString = "ActivationSuccessful";
        private const string ActivationSuccessfulMessageString = "ActivationSuccessfulMessage";
        private const string ActivationFailedString = "ActivationFailed";
        private const string ActivationFailedMessageString = "ActivationFailedMessage";

        private ITranslator BuildTranslator()
        {
            var translationData = Data.CreateDataStorage();
            translationData.SetValue(@"LicenseTab\LicenseExpiresNever", "Never");
            translationData.SetValue(@"LicenseTab\ActivationSuccessful", ActivationSuccessfulString);
            translationData.SetValue(@"LicenseTab\ActivationSuccessfulMessage", ActivationSuccessfulMessageString);
            translationData.SetValue(@"LicenseTab\ActivationFailed", ActivationFailedString);
            translationData.SetValue(@"LicenseTab\ActivationFailedMessage", ActivationFailedMessageString);

            foreach (var licenseStatus in Enum.GetValues(typeof (LicenseStatus)))
            {
                translationData.SetValue(@"LicenseTab\LicenseStatus." + licenseStatus, StringValueAttribute.GetValue(licenseStatus));
            }

            // TODO SectionNameTranslator
            return new BasicTranslator("None", translationData);
        }

        [TestCase("")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAA")]
        [TestCase("!!!!!-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        public void IsValidNewLicenseKey_WithInvalidKey_IsFalse(string testKey)
        {
            var viewModel = BuildViewModel();
            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsFalse(validationResult.IsValid);
        }

        [TestCase("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345")]
        [TestCase("ZZZZZ-YYYYY-DEFGH-DDDDD-12345-67890")]
        public void IsValidNewLicenseKey_WithValidKey_IsTrue(string testKey)
        {
            var viewModel = BuildViewModel();

            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsTrue(validationResult.IsValid);
        }

        private LicenseTabViewModel BuildViewModel()
        {
            return new LicenseTabViewModel(_process, _activationHelper, _translator, _interactionInvoker, _dispatcher);
        }

        [Test]
        public void ActivationValidTill_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            var date = DateTime.Now;
            _activationHelper.Activation.Returns(new Activation { ActivatedTill = date });

            var viewModel = BuildViewModel();

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_WithActivationTillDateMinValue_ReturnsEmptyString()
        {
            _activationHelper.Activation.Returns(new Activation {ActivatedTill = DateTime.MinValue});

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_WithNullActivation_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }

        [Test]
        public void OnlineActivationCommand_CurrentEditionIsNotValid_LicenseCheckerActivationIsNotValid_UpdateEditionWithGivenKeyDoNotSaveNewEditionAndInformUser()
        {
            _activationHelper.Activation.Returns(new Activation());

            _expectedLicenseKey = "given-key";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = true;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });
            var messageInteraction = new MessageInteraction("", "", MessageOptions.OKCancel, MessageIcon.None);
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x.Arg<MessageInteraction>());
            _activationHelper.LicenseStatus.Returns(LicenseStatus.Error);
            
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _activationHelper.DidNotReceive().SaveActivation();
            Assert.AreEqual(_expectedLicenseKey.Replace("-", ""), viewModel.Activation.Key, "Given key not set in updated license");

            Assert.AreEqual(ActivationFailedString, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_CurrentEditionIsNotValid_LicenseCheckerActivationIsValid_SaveNewActivationAndUpdateEditionAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    var inputInteraction = x.Arg<InputInteraction>();
                    inputInteraction.Success = true;
                    inputInteraction.InputText = _expectedLicenseKey;
                });
            _activationHelper
                .When(x => x.ActivateWithoutSavingActivation(_expectedLicenseKey))
                .Do(x =>
                {
                    _activationHelper.IsLicenseValid.Returns(true);
                });
            
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            _interactionInvoker.Received().Invoke(Arg.Any<StoreLicenseForAllUsersInteraction>());        
        }

        [Test]
        public void OnlineActivationCommand_CurrentEditionIsValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewActivationAndDoNotUpdateEditionAndInformUser()
        {
            _activationHelper.Activation.Returns(new Activation());

            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    var inputInteraction = x.Arg<InputInteraction>();
                    inputInteraction.Success = true;
                    inputInteraction.InputText = _expectedLicenseKey;
                });
            MessageInteraction messageInteraction = null;
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x[0] as MessageInteraction);
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _activationHelper.DidNotReceive().SaveActivation();
            Assert.AreEqual(ActivationFailedString, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_CurrentEditionIsValid_LicenseCheckerActivationIsValid_SaveNewActivationAndUpdateEditionAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = true;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });
            var messageInteraction = new MessageInteraction("", "", MessageOptions.OKCancel, MessageIcon.None);
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x[0] as MessageInteraction);

            _activationHelper
                .When(x => x.ActivateWithoutSavingActivation(_expectedLicenseKey))
                .Do(x =>
                {
                    _activationHelper.IsLicenseValid.Returns(true);
                });

            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            var success = viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsTrue(success);
            _interactionInvoker.Received().Invoke(Arg.Any<StoreLicenseForAllUsersInteraction>());
        }

        [Test]
        public void OnlineActivationCommand_IsNotExecutableWhileExcuting()
        {
            _expectedLicenseKey = "given-key";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = true;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });
            var enterLicenseCommandIsExecutable = true;
            var viewModel = BuildViewModel();
            _activationHelper.When(x => x.ActivateWithoutSavingActivation(Arg.Any<string>())).Do(
                x => { enterLicenseCommandIsExecutable = viewModel.OnlineActivationCommand.CanExecute(null); });

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsFalse(enterLicenseCommandIsExecutable);
        }

        [Test]
        public void OnlineActivationCommand_WhenExecuted_CallsLicenseChecker()
        {
            _expectedLicenseKey = "ABCDEF";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = true;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });
            
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => {
                });
            var viewModel = BuildViewModel();
            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _activationHelper.Received().ActivateWithoutSavingActivation(_expectedLicenseKey);
        }

        [Test]
        public void OnlineActivationCommand_WhenExecuted_RaisesPropertyChanged()
        {
            _expectedLicenseKey = "ABCDEF";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = true;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            Assert.Contains(nameof(viewModel.IsCheckingLicense), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseKey), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatus), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusText), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseExpiryDate), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LastActivationTime), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.ActivationValidTill), propertyChangedEvents);
        }

        [Test]
        public void OnlineActivationCommand_WhenKeyIsNull_DoesNotCallLicenseChecker()
        {
            _expectedLicenseKey = null;
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction) x[0]).Success = false;
                    ((InputInteraction) x[0]).InputText = _expectedLicenseKey;
                });

            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            _activationHelper.DidNotReceiveWithAnyArgs().ActivateWithoutSavingActivation("");
        }

        [Test]
        public void OnlineActivationCommand_WithoutActivation_IsNotExecutable()
        {
            _activationHelper.Activation.Returns((Activation)null);

            var viewModel = BuildViewModel();

            Assert.IsFalse(viewModel.OnlineActivationCommand.CanExecute(null));
        }

        [Test]
        public void OnlineActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _activationHelper.Activation.Returns(new Activation());

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OnlineActivationCommand.CanExecute(null));
        }

        [Test]
        public void OfflineActivationCommandExecute_OfflineActivationInteractionContainsLicenseKeyWithDashes()
        {
            _activationHelper.Activation.Returns(new Activation {Key = ValidLicenseKey_Normalized });

            var viewModel = BuildViewModel();

            _interactionInvoker.When(x => x.Invoke(Arg.Any<OfflineActivationInteraction>())).Do(
                x =>
                {
                    var offlineActivationInteraction = x.Arg<OfflineActivationInteraction>();
                    Assert.AreEqual(ValidLicenseKey_Normalized, offlineActivationInteraction.LicenseKey);
                });

            viewModel.OfflineActivationCommand.Execute(null);
        }

        [Test]
        public void OfflineActivationCommandExecute_UserCancelsOfflineActivation_ActivationHelper()
        {
            var viewModel = BuildViewModel();

            _interactionInvoker.When(x => x.Invoke(Arg.Any<OfflineActivationInteraction>())).Do(
                x =>
                {
                    var offlineActivationInteraction = x.Arg<OfflineActivationInteraction>();
                    offlineActivationInteraction.Success = false;
                });

            viewModel.OfflineActivationCommand.Execute(null);

            _activationHelper.DidNotReceive().ActivateOfflineActivationStringFromLicenseServer(Arg.Any<string>());
            _activationHelper.DidNotReceive().SaveActivation();
        }

        [Test]
        public void OfflineActivationCommandExecute_LicenseCheckerOfflineActivationStringFromLicenseServer_GetsCalledWithExpectedOfflineLsa()
        {
            _activationHelper.Activation.Returns(new Activation());

            var viewModel = BuildViewModel();

            const string expectedOfflineLsa = "OfflineLsa";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<OfflineActivationInteraction>())).Do(
                x =>
                {
                    var offlineActivationInteraction = x.Arg<OfflineActivationInteraction>();
                    offlineActivationInteraction.Success = true;
                    offlineActivationInteraction.LicenseServerAnswer = expectedOfflineLsa;
                });

            viewModel.OfflineActivationCommand.Execute(null);

            _activationHelper.Received().ActivateOfflineActivationStringFromLicenseServer(expectedOfflineLsa);
        }

        [Test]
        public void OfflineActivationCommandExecute_LicenseCheckerThrowsFormatException_ActivationKeyIsInteractionKey()
        {
            _activationHelper.ActivateOfflineActivationStringFromLicenseServer(Arg.Any<string>()).Throws(new FormatException());
            const string interactionKey = "interactionKey";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<OfflineActivationInteraction>())).Do(
                x =>
                {
                    var offlineActivationInteraction = x.Arg<OfflineActivationInteraction>();
                    offlineActivationInteraction.Success = true;
                    offlineActivationInteraction.LicenseKey = interactionKey;
                });
            var viewModel = BuildViewModel();

            viewModel.OfflineActivationCommand.Execute(null);

            Assert.AreEqual(interactionKey, viewModel.Activation.Key);
        }

        [Test]
        public void OfflineActivationCommand_WithoutActivation_IsNotExecutable()
        {
            _activationHelper.Activation.Returns((Activation)null);

            var viewModel = BuildViewModel();

            Assert.IsFalse(viewModel.OfflineActivationCommand.CanExecute(null));
        }

        [Test]
        public void OfflineAActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _activationHelper.Activation.Returns(new Activation());

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OfflineActivationCommand.CanExecute(null));
        }

        [Test]
        public void LastActivation_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            _activationHelper.Activation.Returns(new Activation());

            var date = DateTime.Now;
            _activationHelper.Activation.TimeOfActivation = date;

            var viewModel = BuildViewModel();

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_WithNullActivation_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_ForDateTimeMinValue_ReturnsEmptyString()
        {
            _activationHelper.Activation.Returns(new Activation());
            var date = DateTime.MinValue;
            _activationHelper.Activation.TimeOfActivation = date;

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_WithTimeOfActivationOnDateMinValue_ReturnsEmptyString()
        {
            //_edition.Activation.TimeOfActivation = DateTime.MinValue;

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LicenseExpiryDateString_WithActivationNull_ReturnsEmptyString()
        {
            _activationHelper.Activation.Returns((Activation) null);

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLifetimeLicense_ReturnsNever()
        {
            _activationHelper.Activation.Returns(new Activation());
            _activationHelper.Activation.LicenseExpires = new DateTime(2038, 01, 01);

            var viewModel = BuildViewModel();

            Assert.AreEqual("Never", viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLimitedLicense_ReturnsCorrectDate()
        {
            _activationHelper.Activation.Returns(new Activation());
            _activationHelper.Activation.LicenseExpires = DateTime.Now;

            var viewModel = BuildViewModel();

            Assert.AreEqual(DateTime.Today, DateTime.Parse(viewModel.LicenseExpiryDate));
        }

        [Test]
        public void LicenseExpiryDateString_WithMinExpiryDate_ReturnsEmptyString()
        {
            _activationHelper.Activation.Returns(new Activation());
            _activationHelper.Activation.LicenseExpires = DateTime.MinValue;

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseKeyString_WithExpectedLength_IsFormattedWithDashes()
        {
            _activationHelper.Activation.Returns(new Activation());
            _activationHelper.Activation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";

            var viewModel = BuildViewModel();

            Assert.AreEqual(ValidLicenseKey_Normalized, viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithNullStringKey_ReturnsEmptyString()
        {
            //_edition.Activation.Key = null;

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithUnexpectedLength_IsFormattedWithDashes()
        {
            _activationHelper.Activation.Returns(new Activation());
            _activationHelper.Activation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEF";

            var viewModel = BuildViewModel();

            Assert.AreEqual("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-F", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseStatus_ReturnsStatusFromEdition()
        {
            var viewModel = BuildViewModel();

            _activationHelper.LicenseStatus.Returns(LicenseStatus.Blocked);

            Assert.AreEqual(LicenseStatus.Blocked, viewModel.LicenseStatus);
            Assert.AreEqual(StringValueAttribute.GetValue(LicenseStatus.Blocked), viewModel.LicenseStatusText);
        }

        [Test]
        public void ManageLicenseCommandExecute_StartsProcessWithLicenseServerUrl()
        {
            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.ManageLicensesCommand.CanExecute(null));

            viewModel.ManageLicensesCommand.Execute(null);
            _process.Received().Start(Urls.LicenseServerUrl);
        }
    }
}