using System;
using NLog;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class LicenseCondition : IStartupCondition
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsManager _settingsManager;
        private readonly ITranslator _translator;
        private readonly IActivationHelper _activationHelper;
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public LicenseCondition(ISettingsManager settingsManager, ITranslator translator, IActivationHelper activationHelper, IInteractionInvoker interactionInvoker, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
        {
            _settingsManager = settingsManager;
            _translator = translator;
            _activationHelper = activationHelper;
            _interactionInvoker = interactionInvoker;
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
        }

        public StartupConditionResult Check()
        {
            RenewActivation();

            if (_activationHelper.IsLicenseValid)
                return StartupConditionResult.BuildSuccess();

            _logger.Error("Invalid or expired license.");

            var editionWithVersionNumber = _applicationNameProvider.ApplicationName + " " + _versionHelper.FormatWithThreeDigits();

            _settingsManager.LoadAllSettings();
            var settingsProvider = _settingsManager.GetSettingsProvider();

            if (settingsProvider.GpoSettings.HideLicenseTab)
            {
                var errorMessage = _translator.GetFormattedTranslation("Program",
                    "LicenseInvalidGpoHideLicenseTab",
                    editionWithVersionNumber);
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAndHiddenWithGpo, errorMessage);
            }

            var caption = _applicationNameProvider.ApplicationName;
            var message = _translator.GetFormattedTranslation("Program", "LicenseInvalid", editionWithVersionNumber);
            var result = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Exclamation);
            if (result != MessageResponse.Yes)
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAndNotReactivated, "The license is invalid!", showMessage:false);

            var interaction = new LicenseInteraction();
            _interactionInvoker.Invoke(interaction);

            //Check latest edition for validity
            _activationHelper.LoadActivation();

            if (_activationHelper.IsLicenseValid)
                return StartupConditionResult.BuildSuccess();

            return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.LicenseInvalidAfterReactivation, _translator.GetFormattedTranslation("Program", "LicenseInvalidAfterReactivation", _applicationNameProvider.ApplicationName));
        }

        private void RenewActivation()
        {
            _activationHelper.LoadActivation();

            var lastActivation = _activationHelper.Activation;

            if (lastActivation == null)
                return;

            if (lastActivation.ActivationMethod == ActivationMethod.Offline)
                return;

            var remainingActivationTime = lastActivation.ActivatedTill - DateTime.Now;
            if (remainingActivationTime > TimeSpan.FromDays(4))
                return;

            _activationHelper.RenewActivation();
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}