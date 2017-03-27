using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow(WelcomeWindowViewModel welcomeWindowViewModel)
        {
            DataContext = welcomeWindowViewModel;
            InitializeComponent();
        }
    }
}