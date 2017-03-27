using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NSubstitute;
using NUnit.Framework;
using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.LicenseValidator.Interface;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    [TestFixture]
    public class LicenseTabViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            if (Debugger.IsAttached)
                _timeout = TimeSpan.FromMinutes(5);

            _savedActivation = null;
            _expectedLicenseKey = null;
            _activationFromServer = null;

            _process = Substitute.For<IProcessStarter>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _savedActivation.SomeNotNull(LicenseError.NoActivation));
            _licenseChecker.ActivateWithoutSaving(Arg.Any<string>()).Returns(key => _activationFromServer.SomeNotNull(LicenseError.NoActivation));
            _offlineActivator = Substitute.For<IOfflineActivator>();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();

            _dispatcher = new InvokeImmediatelyDispatcher();
        }

        private IProcessStarter _process;
        private IInteractionInvoker _interactionInvoker;

        private string _expectedLicenseKey;

        private TimeSpan _timeout = TimeSpan.FromMilliseconds(150);

        private Activation _savedActivation;
        private Activation _activationFromServer;

        private const string ValidLicenseKey = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";
        private const string ValidLicenseKey_Normalized = "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-FFFFF";

        private LicenseTabTranslation _translation = new LicenseTabTranslation();

        private IDispatcher _dispatcher;
        private ILicenseChecker _licenseChecker;
        private IOfflineActivator _offlineActivator;

        private LicenseTabViewModel BuildViewModel()
        {
            return new LicenseTabViewModel(_process, _licenseChecker, _offlineActivator, _interactionInvoker, _dispatcher, new LicenseTabTranslation());
        }


        private Activation BuildValidActivation(string key)
        {
            var activation = new Activation(acceptExpiredActivation: true);

            activation.ActivatedTill = DateTime.Today.AddDays(7);
            activation.SetResult(Result.OK, "OK");
            activation.Key = key;

            return activation;
        }

        private Activation BuildBlockedActivation(string key)
        {
            var activation = new Activation(acceptExpiredActivation: true);

            activation.ActivatedTill = DateTime.MinValue;
            activation.SetResult(Result.BLOCKED, "Blocked");
            activation.Key = key;

            return activation;
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

        [Test]
        public void ActivationValidTill_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            var date = DateTime.Now;
            _savedActivation = new Activation(acceptExpiredActivation: true) { ActivatedTill = date };

            var viewModel = BuildViewModel();

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_WithActivationTillDateMinValue_ReturnsEmptyString()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true) { ActivatedTill = DateTime.MinValue };

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
        public void SetTranslation_RaiseTranslationAndLicenseStatusTextPropertyChanged()
        {
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.Translation = new LicenseTabTranslation();

            Assert.Contains(nameof(viewModel.Translation), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusText), propertyChangedEvents);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewEditionAndInformUser()
        {
            _savedActivation = null;

            _expectedLicenseKey = "given-key";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });
            var messageInteraction = new MessageInteraction("", "", MessageOptions.OKCancel, MessageIcon.None);
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x.Arg<MessageInteraction>());

            _activationFromServer = new Activation(acceptExpiredActivation: true);
            _activationFromServer.Key = _expectedLicenseKey.Replace("-", "");
            _activationFromServer.SetResult(Result.LICENSE_EXPIRED, "Expired");

            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());

            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsValid_SavesActivation()
        {
            _savedActivation = null;
            _expectedLicenseKey = "given-key";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });
            _activationFromServer = BuildValidActivation(_expectedLicenseKey);
            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.Received().SaveActivation(_activationFromServer);
            Assert.AreEqual(_expectedLicenseKey.ToUpper(), viewModel.LicenseKey);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsValid_SaveNewActivationAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    var inputInteraction = x.Arg<InputInteraction>();
                    inputInteraction.Success = true;
                    inputInteraction.InputText = _expectedLicenseKey;
                });
            _licenseChecker.ActivateWithoutSaving(_expectedLicenseKey).Returns(x => BuildValidActivation(_expectedLicenseKey).Some<Activation, LicenseError>());

            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);
            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            _interactionInvoker.Received().Invoke(Arg.Any<StoreLicenseForAllUsersInteraction>());
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewActivationAndInformUser()
        {
            _savedActivation = null;

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

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());
            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsValid_ReActivationBlocksCurrentKey_SaveNewActivationAndInformUser()
        {
            _savedActivation = BuildValidActivation("saved activation key");
            _activationFromServer = new Activation(true);
            _activationFromServer.Key = "saved activation key";
            _activationFromServer.SetResult(Result.BLOCKED, "Blocked");

            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    var inputInteraction = x.Arg<InputInteraction>();
                    inputInteraction.Success = true;
                    inputInteraction.InputText = "not null or empty";
                });
            MessageInteraction messageInteraction = null;
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x[0] as MessageInteraction);
            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.Received().SaveActivation(_activationFromServer);
            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsValid_ActivationWithNewKeyIsBlocked_DoNotSaveNewActivation()
        {
            _savedActivation = BuildValidActivation("saved activation key");
            _activationFromServer = BuildBlockedActivation("Not the saved activation key");

            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    var inputInteraction = x.Arg<InputInteraction>();
                    inputInteraction.Success = true;
                    inputInteraction.InputText = "not null or empty";
                });

            MessageInteraction messageInteraction = null;
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x[0] as MessageInteraction);
            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.DidNotReceive().SaveActivation(_activationFromServer);
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsValid_SaveNewActivationAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);

            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            var success = viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            Assert.IsTrue(success);
            _licenseChecker.Received().SaveActivation(_activationFromServer);
            _interactionInvoker.Received().Invoke(Arg.Any<StoreLicenseForAllUsersInteraction>());
        }

        [Test]
        public void OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsValid_ShareLicenseForAllUsersDisabled__SaveLicenseAndInformUser()
        {
            _expectedLicenseKey = "not empty";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);

            var viewModel = BuildViewModel();

            viewModel.ShareLicenseForAllUsersEnabled = false; //Important for this test!!!!

            MessageInteraction messageInteraction = null;
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x => messageInteraction = x[0] as MessageInteraction);
            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            _licenseChecker.Received().SaveActivation(_activationFromServer);
            _interactionInvoker.Received().Invoke(Arg.Any<MessageInteraction>());
            _interactionInvoker.DidNotReceive().Invoke(Arg.Any<StoreLicenseForAllUsersInteraction>());
            Assert.AreEqual(_translation.ActivationSuccessful, messageInteraction.Title);
            Assert.AreEqual(_translation.ActivationSuccessfulMessage, messageInteraction.Text);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.PDFForge, messageInteraction.Icon);
        }

        [Test]
        public void OnlineActivationCommand_IsNotExecutableWhileExecuting()
        {
            _expectedLicenseKey = "given-key";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });
            var enterLicenseCommandIsExecutable = true;
            var viewModel = BuildViewModel();
            _licenseChecker.When(x => x.ActivateWithoutSaving(Arg.Any<string>())).Do(
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
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });

            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(
                x =>
                {
                });
            var viewModel = BuildViewModel();
            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);
            _licenseChecker.Received().ActivateWithoutSaving(_expectedLicenseKey);
        }

        [Test]
        public void OnlineActivationCommand_WhenExecuted_RaisesPropertyChanged()
        {
            _expectedLicenseKey = "ABCDEF";
            _interactionInvoker.When(x => x.Invoke(Arg.Any<InputInteraction>())).Do(
                x =>
                {
                    ((InputInteraction)x[0]).Success = true;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.OnlineActivationCommand.Execute(null);

            viewModel.LicenseCheckFinishedEvent.WaitOne(_timeout);

            Assert.Contains(nameof(viewModel.IsCheckingLicense), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseKey), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusForView), propertyChangedEvents);
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
                    ((InputInteraction)x[0]).Success = false;
                    ((InputInteraction)x[0]).InputText = _expectedLicenseKey;
                });

            var viewModel = BuildViewModel();

            viewModel.OnlineActivationCommand.Execute(null);

            _licenseChecker.DidNotReceiveWithAnyArgs().ActivateWithoutSaving("");
        }

        [Test]
        public void OnlineActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _savedActivation = BuildValidActivation(ValidLicenseKey);

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OnlineActivationCommand.CanExecute(null));
        }

        [Test]
        public void OfflineActivationCommandExecute_OfflineActivationInteractionContainsLicenseKeyWithDashes()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true) { Key = ValidLicenseKey_Normalized };

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

            _offlineActivator.DidNotReceive().ActivateOfflineActivationString(Arg.Any<string>());
            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());
        }

        [Test]
        public void OfflineActivationCommandExecute_LicenseCheckerOfflineActivationStringFromLicenseServer_GetsCalledWithExpectedOfflineLsa()
        {
            _savedActivation = null;

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

            _offlineActivator.Received().ValidateOfflineActivationString(expectedOfflineLsa);
        }

        [Test]
        public void OfflineAActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _savedActivation = BuildValidActivation(ValidLicenseKey);

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OfflineActivationCommand.CanExecute(null));
        }

        [Test]
        public void LastActivation_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);

            var date = DateTime.Now;
            _savedActivation.TimeOfActivation = date;

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
            var activation = new Activation(acceptExpiredActivation: true);
            var date = DateTime.MinValue;
            activation.TimeOfActivation = date;

            _savedActivation = activation;

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_WithTimeOfActivationOnDateMinValue_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LicenseExpiryDateString_WithActivationNull_ReturnsEmptyString()
        {
            _savedActivation = null;

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLifetimeLicense_ReturnsNever()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _savedActivation.LicenseExpires = new DateTime(2038, 01, 01);

            var viewModel = BuildViewModel();

            Assert.AreEqual(_translation.Never, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLimitedLicense_ReturnsCorrectDate()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _savedActivation.LicenseExpires = DateTime.Now;

            var viewModel = BuildViewModel();

            Assert.AreEqual(DateTime.Today, DateTime.Parse(viewModel.LicenseExpiryDate));
        }

        [Test]
        public void LicenseExpiryDateString_WithMinExpiryDate_ReturnsEmptyString()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _savedActivation.LicenseExpires = DateTime.MinValue;

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseKeyString_WithExpectedLength_IsFormattedWithDashes()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _savedActivation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";

            var viewModel = BuildViewModel();

            Assert.AreEqual(ValidLicenseKey_Normalized, viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithNullStringKey_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithUnexpectedLength_GetsNormalized()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _savedActivation.Key = "UNeXPECtEDLENgTH";

            var viewModel = BuildViewModel();

            Assert.AreEqual("UNEXP-ECTED-LENGT-H", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseStatus_ReturnsStatusFromEdition()
        {
            _savedActivation = BuildBlockedActivation("Blocked ActivationKey");

            var viewModel = BuildViewModel();

            Assert.AreEqual(LicenseStatusForView.Invalid, viewModel.LicenseStatusForView);
            Assert.AreEqual(_translation.LicenseStatusBlocked, viewModel.LicenseStatusText);
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