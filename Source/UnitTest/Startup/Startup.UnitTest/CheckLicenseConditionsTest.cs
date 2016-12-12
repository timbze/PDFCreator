using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class CheckLicenseConditionsTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IGpoSettings _gpoSettings;
        private IActivationHelper _activationHelper;
        private Activation _activation;

        [SetUp]
        public void Setup()
        {
            _activation = new Activation();
            _activationHelper = Substitute.For<IActivationHelper>();
            _activationHelper.Activation.Returns(x => _activation);
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _gpoSettings = Substitute.For<IGpoSettings>();
        }

        private LicenseCondition BuildCheckLicenseConditions()
        {
            var translator = new SectionNameTranslator();
            var versionHelper = Substitute.For<IVersionHelper>();
            var settingsManager = Substitute.For<ISettingsManager>();
            settingsManager.GetSettingsProvider().GpoSettings.Returns(x => _gpoSettings);

            return new LicenseCondition(settingsManager, translator, _activationHelper, _interactionInvoker, versionHelper, new ApplicationNameProvider("PDFCreator"));
        }

        [Test]
        public void EditionIndicatesValidLicense_Successful()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _activationHelper.IsLicenseValid.Returns(true);

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WithActivationValidForMoreThanFourDays_DoesNotRenew()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _activationHelper.LicenseStatus.Returns(LicenseStatus.Valid);
            _activationHelper.Activation.ActivatedTill = DateTime.Now.AddDays(4).AddMinutes(5);

            licenseCondition.Check();

            _activationHelper.DidNotReceive().RenewActivation();
        }

        [Test]
        public void WithActivationNull_DoesNotRenew()
        {
            _activation = null;
            var licenseCondition = BuildCheckLicenseConditions();

            _activationHelper.LicenseStatus.Returns(LicenseStatus.Valid);

            licenseCondition.Check();

            _activationHelper.DidNotReceive().RenewActivation();
        }

        [Test]
        public void WithOfflineActivation_DoesNotRenew()
        {
            _activation = new Activation();
            _activation.ActivationMethod = ActivationMethod.Offline;
            var licenseCondition = BuildCheckLicenseConditions();

            licenseCondition.Check();

            _activationHelper.DidNotReceive().RenewActivation();
        }

        [Test]
        public void WithActivationValidForLessThanFourDays_RenewsActivation()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _activationHelper.LicenseStatus.Returns(LicenseStatus.Valid);
            _activationHelper.Activation.ActivatedTill = DateTime.Now.AddDays(3);
            _activationHelper.Activation.Key = "AAA-BBB-CCC";

            licenseCondition.Check();

            _activationHelper.Received().RenewActivation();
        }

        [Test]
        public void WithInvalidLicense_AndLicenseManagementDisabledViaGpo_ShowsError()
        {
            _gpoSettings.HideLicenseTab.Returns(true);
            _activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndHiddenWithGpo, result.ExitCode);
        }

        [Test]
        public void WithInvalidLicense_AndLicenseManagementDisabledViaGpo_GivesCorrectExitCode()
        {
            _gpoSettings.HideLicenseTab.Returns(true);
            _activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(_interactionInvoker.ReceivedCalls().Any());
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndHiddenWithGpo, result.ExitCode);
        }

        [Test]
        public void WithInvalidLicense_UserDoesNotWantToRenewLicense_FailsWithLicenseInvalidAndNotReactivated()
        {
            _activationHelper.LicenseStatus.Returns(LicenseStatus.NoLicense);

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
            _activationHelper.LicenseStatus.Returns(LicenseStatus.Error);
            _activationHelper.Activation.Key = "AAA-BBB";

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
            _activationHelper.LicenseStatus.Returns(LicenseStatus.Error);
            _activationHelper.Activation.Key = "AAA-BBB";

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

            _activationHelper.LicenseStatus.Returns(LicenseStatus.Error);
            _activationHelper.Activation.Key = "AAA-BBB";
            _activationHelper
                .When(x => x.LoadActivation())
                .Do(x =>
                {
                    bool wasRenewed =
                        _interactionInvoker.ReceivedCalls()
                            .Any(c => c.GetArguments()[0].GetType() == typeof(LicenseInteraction));
                    // License becomes valid after it was renewed
                    _activationHelper.LicenseStatus.Returns(wasRenewed ? LicenseStatus.Valid : LicenseStatus.Error);
                    _activationHelper.IsLicenseValid.Returns(wasRenewed);
                });

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
