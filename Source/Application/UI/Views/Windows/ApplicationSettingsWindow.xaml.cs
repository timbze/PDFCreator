using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class ApplicationSettingsWindow : Window
    {
        private readonly ITranslator _translator;
        private readonly IUserGuideHelper _userGuideHelper;

        public ApplicationSettingsWindow(ITranslator translator, ApplicationSettingsViewModel applicationSettingsViewModel, IUserGuideHelper userGuideHelper)
        {
            _userGuideHelper = userGuideHelper;
            _translator = translator;
            DataContext = applicationSettingsViewModel;

            InitializeComponent();

            GeneralTabUserControl.PreviewLanguageAction = UpdateTranslations;

            _translator.Translate(this);
        }

        private void UpdateTranslations()
        {
            _translator.Translate(this);
            _translator.Translate(GeneralTabUserControl);
            _translator.Translate(PrinterTabUserControl);
            _translator.Translate(TitleTabUserControl);
            _translator.Translate(DebugTabUserControl);
            _translator.Translate(PdfArchitectTabUserControl);

            var viewModel = DataContext as ApplicationSettingsViewModel;
            viewModel?.ViewModelBundle.PrinterTabViewModel.TranslateProfileNames();
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