using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class CheckLicenseConditionsTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IGpoSettings _gpoSettings;
        private Activation _activation;
        private ILicenseChecker _licenseChecker;

        [SetUp]
        public void Setup()
        {
            _activation = new Activation(acceptExpiredActivation: true);
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _activation.SomeNotNull(LicenseError.NoActivation));
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _gpoSettings = Substitute.For<IGpoSettings>();
        }

        private LicenseCondition BuildCheckLicenseConditions()
        {
            var translation = new ApplicationTranslation();
            var versionHelper = Substitute.For<IVersionHelper>();
            var settingsManager = Substitute.For<ISettingsManager>();
            settingsManager.GetSettingsProvider().GpoSettings.Returns(x => _gpoSettings);

            return new LicenseCondition(settingsManager, new ProgramTranslation(), _licenseChecker, _interactionInvoker, versionHelper, new ApplicationNameProvider("PDFCreator"));
        }

        private Activation BuildValidActivation()
        {
            var activation = new Activation(acceptExpiredActivation: true);

            activation.ActivatedTill = DateTime.Today.AddDays(7);
            activation.SetResult(Result.OK, "OK");

            return activation;
        }

        [Test]
        public void ValidActivation_Successful()
        {
            _activation = BuildValidActivation();
            var licenseCondition = BuildCheckLicenseConditions();

            // TODO fix all comments!!!!!!!
            //_activationHelper.IsLicenseValid.Returns(true);

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WithActivationValidForMoreThanFourDays_DoesNotRenew()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            //_activationHelper.LicenseStatus.Returns(LicenseStatus.Valid);
            _activation.ActivatedTill = DateTime.Now.AddDays(4).AddMinutes(5);

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithActivationNull_DoesNotRenew()
        {
            _activation = null;
            var licenseCondition = BuildCheckLicenseConditions();

            //_activationHelper.LicenseStatus.Returns(LicenseStatus.Valid);

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithOfflineActivation_DoesNotRenew()
        {
            _activation.ActivationMethod = ActivationMethod.Offline;
            var licenseCondition = BuildCheckLicenseConditions();

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithActivationValidForLessThanFourDays_RenewsActivation()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _activation.ActivatedTill = DateTime.Now.AddDays(3);
            _activation.Key = "AAA-BBB-CCC";

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.Received().ActivateWithKey(_activation.Key);
        }

        [Test]
        public void WithInvalidLicense_AndLicenseManagementDisabledViaGpo_ShowsError()
        {
            _gpoSettings.HideLicenseTab.Returns(true);
            //_activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndHiddenWithGpo, result.ExitCode);
        }

        [Test]
        public void WithInvalidLicense_AndLicenseManagementDisabledViaGpo_GivesCorrectExitCode()
        {
            _gpoSettings.HideLicenseTab.Returns(true);
            //_activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(_interactionInvoker.ReceivedCalls().Any());
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndHiddenWithGpo, result.ExitCode);
        }

        [Test]
        public void WithInvalidLicense_UserDoesNotWantToRenewLicense_FailsWithLicenseInvalidAndNotReactivated()
        {
            //_activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var tmpInteraction = x.Arg<MessageInteraction>();
                    tmpInteraction.Response = MessageResponse.No;
                });

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndNotReactivated, result.ExitCode);
        }

        [Test]
        public void WithInvalidLicense_UserWantsToRenew_LicenseInteractionIsTriggeredWithKey()
        {
            //_activationHelper.LicenseStatus.Returns(LicenseStatus.Error);
            _activation.Key = "AAA-BBB";

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var tmpInteraction = x.Arg<MessageInteraction>();
                    tmpInteraction.Response = MessageResponse.Yes;
                });

            var licenseCondition = BuildCheckLicenseConditions();

            licenseCondition.Check();

            _interactionInvoker.Received().Invoke(Arg.Any<LicenseInteraction>());
        }

        [Test]
        public void AfterRenewingLicense_LicenseStillInvalid_FailsWithLicenseInvalidAfterReactivation()
        {
            _activation.Key = "AAA-BBB";

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var tmpInteraction = x.Arg<MessageInteraction>();
                    tmpInteraction.Response = MessageResponse.Yes;
                });

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.LicenseInvalidAfterReactivation, result.ExitCode);
        }

        [Test]
        public void AfterRenewingLicense_LicenseIsInvalid_Success()
        {
            // When asked to renew the activation, we'll answer 'Yes"
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var messageInteraction = x.Arg<MessageInteraction>();
                    if (messageInteraction.Buttons == MessageOptions.YesNo)
                        messageInteraction.Response = MessageResponse.Yes;
                });

            _activation.Key = "AAA-BBB";
            // Activation is valid after LicenseInteraction was shown
            _licenseChecker
                .When(x => x.ActivateWithKey(_activation.Key))
                .Do(x =>
                {
                    _activation = BuildValidActivation();
                });

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
