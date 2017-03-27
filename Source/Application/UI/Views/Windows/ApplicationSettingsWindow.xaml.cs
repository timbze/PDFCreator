using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using Translatable;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class ApplicationSettingsWindow : Window
    {
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly TranslationFactory _translationFactory;
        private ApplicationSettingsViewModel _applicationSettingsViewModel;

        public ApplicationSettingsWindow( ApplicationSettingsViewModel applicationSettingsViewModel, IUserGuideHelper userGuideHelper, TranslationFactory translationFactory)
        {
            _userGuideHelper = userGuideHelper;
            _translationFactory = translationFactory;
            DataContext = applicationSettingsViewModel;
            _applicationSettingsViewModel = applicationSettingsViewModel;

            InitializeComponent();

            GeneralTabUserControl.PreviewLanguageAction = UpdateTranslations;
        }

        private void UpdateTranslations()
        {
            var vms = _applicationSettingsViewModel.ViewModelBundle;
            _applicationSettingsViewModel.Translation = _translationFactory.CreateTranslation<ApplicationSettingsWindowTranslation>();
            vms.GeneralTabViewModel.Translation = _translationFactory.CreateTranslation<GeneralTabTranslation>();
            vms.PrinterTabViewModel.Translation = _translationFactory.CreateTranslation<PrinterTabTranslation>();
            vms.TitleTabViewModel.Translation = _translationFactory.CreateTranslation<TitleTabTranslation>();
            vms.DebugTabViewModel.Translation = _translationFactory.CreateTranslation<DebugTabTranslation>();
            vms.LicenseTabViewModel.Translation = _translationFactory.CreateTranslation<LicenseTabTranslation>();
            vms.PdfArchitectTabViewModel.Translation = _translationFactory.CreateTranslation<PdfArchitectTabTranslation>();

            vms.PrinterTabViewModel.TranslateProfileNames();
        }

        private void ShowConextBasedHelp()
        {
            var active = TabControl.SelectedItem as TabItem;
            if (ReferenceEquals(active, GeneralTab))
                _userGuideHelper.ShowHelp(HelpTopic.AppGeneral);
            else if (ReferenceEquals(active, PrinterTab))
                _userGuideHelper.ShowHelp(HelpTopic.AppPrinters);
            else if (ReferenceEquals(active, TitleTab))
                _userGuideHelper.ShowHelp(HelpTopic.AppTitle);
            else if (ReferenceEquals(active, DebugTab))
                _userGuideHelper.ShowHelp(HelpTopic.AppDebug);
            else if (ReferenceEquals(active, LicenseTab))
                _userGuideHelper.ShowHelp(HelpTopic.AppLicense);
            else
                _userGuideHelper.ShowHelp(HelpTopic.AppGeneral);
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowConextBasedHelp();
        }

        private void ApplicationSettingsWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                ShowConextBasedHelp();
        }
    }
}