using System;
using System.Windows;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class ApplicationSettingsViewModel : InteractionAwareViewModelBase<ApplicationSettingsInteraction>
    {
        private readonly TranslationHelper _translationHelper;
        private readonly LicenseOptionProvider _licenseOption;

        public EventHandler SettingsChanged;

        public ApplicationSettingsViewModel(ApplicationSettingsViewModelBundle viewModelBundle, TranslationHelper translationHelper, LicenseOptionProvider licenseOption)
        {
            _translationHelper = translationHelper;
            _licenseOption = licenseOption;
            ViewModelBundle = viewModelBundle;

            SaveSettingsCommand = new DelegateCommand(SaveSettingsExecute);
            ClosingCommand = new DelegateCommand(OnClosing);

            viewModelBundle.DebugTabViewModel.SettingsLoaded += (sender, args) =>
            {
                Interaction.Settings = args.Settings;
                HandleInteractionObjectChanged();
            };
        }

        public ApplicationSettingsViewModelBundle ViewModelBundle { get; }

        public DelegateCommand SaveSettingsCommand { get; set; }
        public DelegateCommand ClosingCommand { get; set; }



        public ApplicationSettings ApplicationSettings { get; private set; }

        public IGpoSettings GpoSettings { get; private set; }

        public bool TitleTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableTitleTab;
            }
        }

        public bool DebugTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableDebugTab;
            }
        }

        public Visibility LicenseTabVisibility
        {
            get
            {
                if (_licenseOption.HideLicenseOptions)
                    return Visibility.Collapsed;

                if (GpoSettings == null)
                    return Visibility.Visible;

                return GpoSettings.HideLicenseTab ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool PrinterTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisablePrinterTab;
            }
        }

        public Visibility PdfArchitectVisibilty
        {
            get
            {
                if (GpoSettings == null)
                    return Visibility.Visible;
                return GpoSettings.HidePdfArchitectInfo ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void OnClosing(object obj)
        {
            _translationHelper.RevertTemporaryTranslation();
        }

        private void SaveSettingsExecute(object obj)
        {
            Interaction.Success = true;
            FinishInteraction();
        }

        private void OnSettingsChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(ApplicationSettings));
            RaiseGpoPropertiesChanged();

            SettingsChanged?.Invoke(this, e);
        }

        protected override void HandleInteractionObjectChanged()
        {
            GpoSettings = Interaction.GpoSettings;

            ViewModelBundle.GeneralTabViewModel.SetSettingsAndRaiseNotifications(Interaction.Settings, Interaction.GpoSettings);
            ViewModelBundle.PrinterTabViewModel.SetSettingsAndRaiseNotifications(Interaction.Settings, Interaction.GpoSettings);
            ViewModelBundle.DebugTabViewModel.SetSettingsAndRaiseNotifications(Interaction.Settings.ApplicationSettings, Interaction.GpoSettings);
            // TODO LicenseTabViewModel ?

            ViewModelBundle.TitleTabViewModel.ApplyTitleReplacements(Interaction.Settings.ApplicationSettings.TitleReplacement);

            OnSettingsChanged(EventArgs.Empty);
        }

        private void RaiseGpoPropertiesChanged()
        {
            RaisePropertyChanged(nameof(GpoSettings));
            RaisePropertyChanged(nameof(TitleTabIsEnabled));
            RaisePropertyChanged(nameof(DebugTabIsEnabled));
            RaisePropertyChanged(nameof(PrinterTabIsEnabled));
            RaisePropertyChanged(nameof(PdfArchitectVisibilty));
            RaisePropertyChanged(nameof(LicenseTabVisibility));
        }
    }
}