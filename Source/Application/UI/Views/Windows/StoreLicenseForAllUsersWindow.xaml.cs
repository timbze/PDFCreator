using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class StoreLicenseForAllUsersWindow : Window
    {
        public StoreLicenseForAllUsersWindow(StoreLicenseForAllUsersWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
