using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class GeneralTabViewModel : ObservableObject
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IOsHelper _osHelper;
        private readonly IProcessStarter _processStarter;
        private readonly TranslationHelper _translationHelper;
        private readonly IUacAssistant _uacAssistant;
        private IList<Language> _languages;

        public GeneralTabViewModel(ILanguageProvider languageProvider, TranslationHelper translationHelper,
            ITranslator translator,
            IUpdateAssistant updateAssistant, IUacAssistant uacAssistant, IInteractionInvoker invoker,
            IOsHelper osHelper, IProcessStarter processStarter)
        {
            Translator = translator;
            _updateAssistant = updateAssistant;
            _translationHelper = translationHelper;
            _uacAssistant = uacAssistant;
            _invoker = invoker;
            _osHelper = osHelper;
            _processStarter = processStarter;
            Languages = languageProvider.GetAvailableLanguages().ToList();

            UpdateCheckCommand = new DelegateCommand(ExecuteUpdateCheck);
            PreviewTranslationCommand = new DelegateCommand(ExecutePreviewTranslation);

            OnSettingsChanged();
        }

        public ITranslator Translator { get; }
        private readonly IUpdateAssistant _updateAssistant;
        private IList<ConversionProfile> _conversionProfiles = new List<ConversionProfile>();

        public ICommand AddExplorerIntegrationCommand
            => new DelegateCommand(o => _uacAssistant.AddExplorerIntegration());

        public ICommand RemoveExplorerIntegrationCommand
            => new DelegateCommand(o => _uacAssistant.RemoveExplorerIntegration());

        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);

        private void VisitWebsiteExecute(object o)
        {
            try
            {
                _processStarter.Start(Urls.PdfforgeWebsiteUrl);
            }
            catch
            {
                // ignored
            }
        }

        public ICommand UpdateCheckCommand { get; }
        public ICommand PreviewTranslationCommand { get; }

        public Visibility RequiresUacVisibility
        {
            get { return _osHelper.UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Conversion.Settings.ApplicationSettings ApplicationSettings { get; private set; }

        public IGpoSettings GpoSettings { get; private set; }

        public ApplicationProperties ApplicationProperties { get; private set; }

        public IList<Language> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                RaisePropertyChanged(nameof(Languages));
            }
        }

        public bool DisplayUpdateWarning
        {
            get
            {
                if (ApplicationSettings == null)
                    return false;
                return ApplicationSettings.UpdateInterval == UpdateInterval.Never;
            }
        }

        public IEnumerable<AskSwitchPrinter> AskSwitchPrinterValues
        {
            get
            {
                return new List<AskSwitchPrinter>
                {
                    new AskSwitchPrinter(
                        Translator.GetTranslation("ApplicationSettingsWindow", "Ask"), true),
                    new AskSwitchPrinter(
                        Translator.GetTranslation("ApplicationSettingsWindow", "Yes"), false)
                };
            }
        }

        public IEnumerable<EnumValue<UpdateInterval>> UpdateIntervals
        {
            get { return Translator.GetEnumTranslation<UpdateInterval>(); }
        }

        public bool LanguageIsEnabled
        {
            get
            {
                if (ApplicationSettings == null)
                    return true;

                return GpoSettings?.Language == null;
            }
        }

        public string CurrentLanguage
        {
            get
            {
                if (ApplicationSettings == null)
                    return null;

                if ((GpoSettings == null) || (GpoSettings.Language == null))
                    return ApplicationSettings.Language;
                return GpoSettings.Language;
            }
            set { ApplicationSettings.Language = value; }
        }

        public bool UpdateIsEnabled
        {
            get { return GpoSettings?.UpdateInterval == null; }
        }

        public UpdateInterval CurrentUpdateInterval
        {
            get
            {
                if (ApplicationSettings == null)
                    return UpdateInterval.Weekly;

                if (GpoSettings?.UpdateInterval == null)
                    return ApplicationSettings.UpdateInterval;
                return UpdateIntervalHelper.ParseUpdateInterval(GpoSettings.UpdateInterval);
            }
            set
            {
                ApplicationSettings.UpdateInterval = value;
                RaisePropertyChanged(nameof(UpdateInterval));
                RaisePropertyChanged(nameof(DisplayUpdateWarning));
            }
        }

        public Visibility UpdateCheckControlVisibility
            => _updateAssistant.UpdatesEnabled ? Visibility.Visible : Visibility.Collapsed;

        public event EventHandler PreviewTranslation;

        private void ExecutePreviewTranslation(object o)
        {
            _translationHelper.SetTemporaryTranslation(Languages.First(l => l.CommonName == CurrentLanguage));
            Translator.Translate(this);
            _translationHelper.TranslateProfileList(_conversionProfiles);

            // Notify about changed properties
            RaisePropertyChanged(nameof(UpdateIntervals));
            RaisePropertyChanged(nameof(CurrentUpdateInterval));
            RaisePropertyChanged(nameof(AskSwitchPrinterValues));
            RaisePropertyChanged(nameof(ApplicationSettings));

            PreviewTranslation?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteUpdateCheck(object o)
        {
            if (!_updateAssistant.UpdateProcedureIsRunning)
            {
                _updateAssistant.UpdateProcedure(false, false);
            }
            else
            {
                var message = Translator.GetTranslation("UpdateManager",
                    "UpdateCheckIsRunning");
                var caption = Translator.GetTranslation("UpdateManager",
                    "PDFCreatorUpdate");

                var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Info);
                _invoker.Invoke(interaction);
            }
        }

        public void SetSettingsAndRaiseNotifications(PdfCreatorSettings settings, IGpoSettings gpoSettings)
        {
            _conversionProfiles = settings.ConversionProfiles;
            ApplicationSettings = settings.ApplicationSettings;
            ApplicationProperties = settings.ApplicationProperties;
            GpoSettings = gpoSettings;

            OnSettingsChanged();
        }

        private void OnSettingsChanged()
        {
            RaisePropertyChanged(nameof(ApplicationSettings));

            RaisePropertyChanged(nameof(ApplicationProperties));
            RaisePropertyChanged(nameof(GpoSettings));
            RaisePropertyChanged(nameof(CurrentLanguage));
            RaisePropertyChanged(nameof(LanguageIsEnabled));
            RaisePropertyChanged(nameof(CurrentUpdateInterval));
            RaisePropertyChanged(nameof(UpdateIsEnabled));
            RaisePropertyChanged(nameof(Languages));
        }
    }
}