using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class StoreLicenseForAllUsersWindow : Window
    {
        public StoreLicenseForAllUsersWindow(StoreLicenseForAllUsersWindowViewModel viewModel, ITranslator translator)
        {
            DataContext = viewModel;
            InitializeComponent();
            translator.Translate(this);
        }
    }
}
