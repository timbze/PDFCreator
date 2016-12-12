using System;
using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class MainWindow : Window
    {
        private readonly ITranslator _translator;

        public MainWindow(MainWindowViewModel viewModel, ITranslator translator, ViewCustomization customization)
        {
            _translator = translator;
            viewModel.TranslationChanged += (sender, args) => _translator.Translate(this);
            DataContext = viewModel;
            
            InitializeComponent();

            // Apply company name for customized setups
            ApplyCustomization(customization);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _translator.Translate(this);

            ((MainWindowViewModel) DataContext).WelcomeCommand.Execute(null);
        }

        private void ApplyCustomization(ViewCustomization customization)
        {
            if (customization.ApplyCustomization)
            {
                LicensedForText.Visibility = Visibility.Visible;
                LicensedForText.Text = customization.MainWindowText;
            }
            else
            {
                LicensedForText.Visibility = Visibility.Hidden;
            }
        }
    }
}