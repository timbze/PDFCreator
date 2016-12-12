using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow(WelcomeWindowViewModel welcomeWindowViewModel, ITranslator translator)
        {
            DataContext = welcomeWindowViewModel;
            InitializeComponent();
            translator.Translate(this);
        }
    }
}