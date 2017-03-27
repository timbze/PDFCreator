using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class UpdateDownloadWindow : Window
    {
        public UpdateDownloadWindow(UpdateDownloadWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}