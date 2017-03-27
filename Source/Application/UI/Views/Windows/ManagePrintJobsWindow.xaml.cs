using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class ManagePrintJobsWindow : Window
    {
        public ManagePrintJobsWindow(ManagePrintJobsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}