using NSubstitute;
using NUnit.Framework;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class CheckLicenseConditionsTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IGpoSettings _gpoSettings;
        private Activation _savedActivation;
        private ILicenseChecker _licenseChecker;
        private string _applicationName = "PDFCreator Free";
        private string _versionWithTreeDigits = "0.0.0";
        private string _editionWithVersionNumber;
        private ProgramTranslation _translation;

        [SetUp]
        public void Setup()
        {
            _savedActivation = new Activation(acceptExpiredActivation: true);
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _savedActivation.SomeNotNull(LicenseError.NoActivation));
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _gpoSettings = Substitute.For<IGpoSettings>();
            _translation = new ProgramTranslation();

            _editionWithVersionNumber = _applicationName + " " + _versionWithTreeDigits;
        }

        private LicenseCondition BuildCheckLicenseConditions()
        {
            var versionHelper = Substitute.For<IVersionHelper>();
            versionHelper.FormatWithThreeDigits().Returns(_versionWithTreeDigits);
            var applicationNameProvider = new ApplicationNameProvider("Free");

            return new LicenseCondition(new TranslationFactory(), _licenseChecker, _interactionInvoker, versionHelper, applicationNameProvider, _gpoSettings);
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
            _savedActivation = BuildValidActivation();
            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WithActivationValidForMoreThanFourDays_DoesNotRenew()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _savedActivation = BuildValidActivation();
            _savedActivation.ActivatedTill = DateTime.Now.AddDays(4).AddMinutes(5);

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithActivationNull_DoesNotRenew()
        {
            _savedActivation = null;
            var licenseCondition = BuildCheckLicenseConditions();

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithOfflineActivation_DoesNotRenew()
        {
            _savedActivation.ActivationMethod = ActivationMethod.Offline;
            var licenseCondition = BuildCheckLicenseConditions();

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.DidNotReceive().ActivateWithKey(Arg.Any<string>());
        }

        [Test]
        public void WithActivationValidForLessThanFourDays_RenewsActivation()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _savedActivation.ActivatedTill = DateTime.Now.AddDays(3);
            _savedActivation.Key = "AAA-BBB-CCC";

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.Received().ActivateWithKey(_savedActivation.Key);
        }

        [Test]
        public void WithInvalidLicense_AndLicenseManagementDisabledViaGpo_ResultIsNotSuccesfullWithCorrespondingErrorAndWithoutInteraction()
        {
            _savedActivation = null;
            _gpoSettings.HideLicenseTab.Returns(true);

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsFalse(_interactionInvoker.ReceivedCalls().Any());
            Assert.AreEqual((int)ExitCode.LicenseInvalidAndHiddenWithGpo, result.ExitCode);
            Assert.AreEqual(_translation.GetFormattedLicenseInvalidGpoHideLicenseTab(_editionWithVersionNumber), result.Message);
        }

        [Test]
        public void WithInvalidLicense_UserDoesNotWantToRenewLicense_FailsWithLicenseInvalidAndNotReactivated()
        {
            _savedActivation = null;
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
            Assert.AreEqual("The license is invalid!", result.Message);
            Assert.IsFalse(result.ShowMessage);
        }

        [Test]
        public void WithInvalidLicense_UserWantsToRenew_LicenseInteractionIsTriggered()
        {
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
            Assert.AreEqual(_translation.GetFormattedLicenseInvalidAfterReactivationTranslation(_applicationName), result.Message);
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

            // Activation is valid after LicenseInteraction was shown
            _licenseChecker
                .When(x => x.ActivateWithKey(_savedActivation.Key))
                .Do(x =>
                {
                    _savedActivation = BuildValidActivation();
                });

            var licenseCondition = BuildCheckLicenseConditions();

            var result = licenseCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WithActivationWithVersionMismatch_RenewsActivation()
        {
            var licenseCondition = BuildCheckLicenseConditions();

            _savedActivation.ActivatedTill = DateTime.Now.AddDays(7);
            _savedActivation.Key = "AAA-BBB-CCC";
            _savedActivation.SetResult(Result.VERSION_MISMATCH, "Version mismatch");

            licenseCondition.Check();

            _licenseChecker.DidNotReceive().GetActivation();
            _licenseChecker.Received().ActivateWithKey(_savedActivation.Key);
        }
    }
}
