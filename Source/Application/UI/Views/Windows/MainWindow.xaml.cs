using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel, ViewCustomization customization)
        {
            DataContext = viewModel;
            
            InitializeComponent();

            // Apply company name for customized setups
            ApplyCustomization(customization);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
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