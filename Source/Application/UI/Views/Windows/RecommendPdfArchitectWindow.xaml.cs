using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class RecommendPdfArchitectWindow : Window
    {
        public RecommendPdfArchitectWindow(RecommendPdfArchitectWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}