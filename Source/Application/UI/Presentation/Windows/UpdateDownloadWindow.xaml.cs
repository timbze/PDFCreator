using pdfforge.PDFCreator.UI.Presentation.Windows;
using System.Windows;

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
