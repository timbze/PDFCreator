using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
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
            IUpdateAssistant updateAssistant, IUacAssistant uacAssistant, IInteractionInvoker invoker,
            IOsHelper osHelper, IProcessStarter processStarter, GeneralTabTranslation translation)
        {
            _translation = translation;
            _updateAssistant = updateAssistant;
            _translationHelper = translationHelper;
            _uacAssistant = uacAssistant;
            _invoker = invoker;
            _osHelper = osHelper;
            _processStarter = processStarter;
            Languages = languageProvider.GetAvailableLanguages().ToList();
            AddExplorerIntegrationCommand = new DelegateCommand(ExecuteAddToExplorerContextMenu);
            RemoveExplorerIntegrationCommand = new DelegateCommand(ExecuteFromExplorerContextMenu);

            UpdateCheckCommand = new DelegateCommand(ExecuteUpdateCheck);
            PreviewTranslationCommand = new DelegateCommand(ExecutePreviewTranslation);

            OnSettingsChanged();
        }

        private readonly IUpdateAssistant _updateAssistant;
        private IList<ConversionProfile> _conversionProfiles = new List<ConversionProfile>();
        private GeneralTabTranslation _translation;

        public ICommand AddExplorerIntegrationCommand { get; set; }

        public ICommand RemoveExplorerIntegrationCommand { get; set; }

        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);

        public string PdfforgeWebsiteUrl => Urls.PdfforgeWebsiteUrl;

        public GeneralTabTranslation Translation
        {
            get { return _translation; }
            set { _translation = value;
                RaisePropertyChanged(nameof(Translation));
                RaisePropertyChanged(nameof(AskSwitchPrinterValues));
                RaisePropertyChanged(nameof(ApplicationSettings));
                RaisePropertyChanged(nameof(CurrentUpdateInterval));
            }
        }

        private void VisitWebsiteExecute(object o)
        {
            try
            {
                _processStarter.Start(PdfforgeWebsiteUrl);
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

        public bool IsAddedToExplorer { get; private set; }
        public bool IsRemovedFromExplorer { get; private set; }

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
                    new AskSwitchPrinter(Translation.Ask, true),
                    new AskSwitchPrinter(Translation.Yes, false)
                };
            }
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
            var tmpLanguage = Languages.First(l => l.Iso2 == CurrentLanguage);
            _translationHelper.SetTemporaryTranslation(tmpLanguage);
            _translationHelper.TranslateProfileList(_conversionProfiles);

            // Notify about changed properties
            RaisePropertyChanged(nameof(CurrentUpdateInterval));
            RaisePropertyChanged(nameof(AskSwitchPrinterValues));
            RaisePropertyChanged(nameof(ApplicationSettings));
            RaisePropertyChanged(nameof(IsRemovedFromExplorer));
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
                var message = Translation.UpdateCheckIsRunning;
                var caption = Translation.UpdateCheckTitle;

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

        private void ExecuteAddToExplorerContextMenu(object obj)
        {
            IsAddedToExplorer = false;
            RaisePropertyChanged(nameof(IsAddedToExplorer));

            IsAddedToExplorer = _uacAssistant.AddExplorerIntegration();
            RaisePropertyChanged(nameof(IsAddedToExplorer));
        }

        private void ExecuteFromExplorerContextMenu(object obj)
        {
            IsRemovedFromExplorer = false;
            RaisePropertyChanged(nameof(IsRemovedFromExplorer));

            IsRemovedFromExplorer = _uacAssistant.RemoveExplorerIntegration();
            RaisePropertyChanged(nameof(IsRemovedFromExplorer));
        }
    }
}