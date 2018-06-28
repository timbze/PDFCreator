using NLog;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class LicenseCondition : IStartupCondition
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ProgramTranslation _translation;
        private readonly ILicenseChecker _licenseChecker;
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IGpoSettings _gpoSettings;

        public bool CanRequestUserInteraction => true;

        public LicenseCondition(ITranslationFactory translationFactory, ILicenseChecker licenseChecker, IInteractionInvoker interactionInvoker, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider, IGpoSettings gpoSettings)
        {
            _translation = translationFactory.CreateTranslation<ProgramTranslation>();
            _licenseChecker = licenseChecker;
            _interactionInvoker = interactionInvoker;
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
            _gpoSettings = gpoSettings;
        }

        public StartupConditionResult Check()
        {
            var activation = RenewActivation();

            if (activation.Exists(a => a.IsActivationStillValid()))
                return StartupConditionResult.BuildSuccess();

            _logger.Error("Invalid or expired license.");

            var editionWithVersionNumber = _applicationNameProvider.ApplicationNameWithEdition + " " + _versionHelper.FormatWithThreeDigits();

            if (_gpoSettings.HideLicenseTab)
            {
                var errorMessage = _translation.GetFormattedLicenseInvalidGpoHideLicenseTab(editionWithVersionNumber);
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAndHiddenWithGpo, errorMessage);
            }

            var caption = _applicationNameProvider.ApplicationName;
            var message = _translation.GetFormattedLicenseInvalidTranslation(editionWithVersionNumber);
            var result = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Exclamation);
            if (result != MessageResponse.Yes)
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAndNotReactivated, "The license is invalid!", showMessage: false);

            var interaction = new LicenseInteraction();
            _interactionInvoker.Invoke(interaction);

            //Check latest edition for validity
            activation = _licenseChecker.GetSavedActivation();

            if (activation.Exists(a => a.IsActivationStillValid()))
                return StartupConditionResult.BuildSuccess();

            return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAfterReactivation, _translation.GetFormattedLicenseInvalidAfterReactivationTranslation(_applicationNameProvider.ApplicationNameWithEdition));
        }

        private Option<Activation, LicenseError> RenewActivation()
        {
            var activation = _licenseChecker.GetSavedActivation();

            if (activation.Exists(a => a.ActivationMethod == ActivationMethod.Offline))
                return activation;

            if (activation.Exists(IsActivationPeriodStillValid))
                return activation;

            var licenseKey = activation.Match(
                some: a => a.Key.Some<string, LicenseError>(),
                none: e => _licenseChecker.GetSavedLicenseKey());

            return licenseKey.Match(
                some: key =>
                {
                    var newActivation = _licenseChecker.ActivateWithoutSaving(key);

                    // Only save if we receive a valid license or the license was blocked
                    newActivation
                    .Filter(a => a.IsActivationStillValid() || a.Result == Result.BLOCKED, LicenseError.NoActivation)
                    .MatchSome(a => _licenseChecker.SaveActivation(a));

                    // If the online activation failed and the old activation is still valid, return the current activation
                    if (!newActivation.HasValue && activation.Exists(a => a.IsActivationStillValid()))
                        return activation;

                    return newActivation;
                },
                none: e => Option.None<Activation, LicenseError>(LicenseError.NoLicenseKey));
        }

        private bool IsActivationPeriodStillValid(Activation activation)
        {
            var remainingActivationTime = activation.ActivatedTill - DateTime.UtcNow;
            return activation.IsActivationStillValid() && remainingActivationTime >= TimeSpan.FromDays(4);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
