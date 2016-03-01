using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Assistants;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Assistants;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Views;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.ViewModels.UserControls;

namespace pdfforge.PDFCreator.Views.UserControls
{
    internal partial class GeneralTab : UserControl
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;

        public GeneralTab()
        {
            InitializeComponent();
            if (TranslationHelper.IsInitialized)
            {
                TranslationHelper.TranslatorInstance.Translate(this);
            }
        }

        public Action PreviewLanguageAction { private get; set; }

        public GeneralTabViewModel ViewModel
        {
            get { return (GeneralTabViewModel) DataContext; }
        }

        public Visibility RequiresUacVisibility
        {
            get { return new OsHelper().UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        private void LanguagePreviewButton_Click(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.SetTemporaryTranslation((Language)LanguageBox.SelectionBoxItem);
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
            TranslationHelper.Instance.TranslateProfileList(SettingsHelper.Settings.ConversionProfiles);
            //overwrite items of comboboxes with translated items  
            UpdateIntervalComboBox.ItemsSource = ViewModel.UpdateIntervals;
            UpdateIntervalComboBox.SelectedValue = ViewModel.ApplicationSettings.UpdateInterval;
            ChangeDefaultPrinterComboBox.ItemsSource = ViewModel.AskSwitchPrinterValues;
            ChangeDefaultPrinterComboBox.SelectedValue = ViewModel.ApplicationSettings.AskSwitchDefaultPrinter;
            PreviewLanguageAction();
        }

        private void DownloadHyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(Urls.PdfforgeWebsiteUrl);
            e.Handled = true;
        }

        private void UpdateCheckButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!UpdateAssistant.Instance.UpdateProcedureIsRunning)
            {
                UpdateAssistant.Instance.Initialize(ViewModel.ApplicationSettings, ViewModel.ApplicationProperties, SettingsHelper.GpoSettings);
                UpdateAssistant.Instance.NewVersionEvent += UpdateAssistant.LaunchNewUpdateForm;
                UpdateAssistant.Instance.NoNewVersionEvent += UpdateAssistant.LaunchNoUpdateForm;
                UpdateAssistant.Instance.ErrorEvent += UpdateAssistant.LaunchErrorForm;
                UpdateAssistant.Instance.FinishedEvent += StoreNextUpdateAndEnableBtn;
                UpdateAssistant.Instance.UpdateProcedure(false);
            }
            else
            {
                var message = TranslationHelper.Instance.TranslatorInstance.GetTranslation("UpdateManager",
                    "UpdateCheckIsRunning", "Update-Check is already running.");
                var caption = TranslationHelper.Instance.TranslatorInstance.GetTranslation("UpdateManager",
                    "PDFCreatorUpdate", "PDFCreator Update");
                MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Info);
            }
        }

        private void StoreNextUpdateAndEnableBtn(object sender, UpdateEventArgs eventArgs)
        {
            Dispatcher.BeginInvoke(
                new ThreadStart(() =>
                {
                    ViewModel.ApplicationProperties.NextUpdate = UpdateAssistant.NextUpdate;
                }));
        }

        private void RemoveExplorerIntegrationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var uac = new UacAssistant();
            uac.RemoveExplorerIntegration();
        }

        private void AddExplorerIntegrationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var uac = new UacAssistant();
            uac.AddExplorerIntegration();
        }

        private void UpdateIntervalComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.UpdateIntervalChanged();
        }
    }
}
