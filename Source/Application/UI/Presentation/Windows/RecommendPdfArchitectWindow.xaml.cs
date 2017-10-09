using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
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
