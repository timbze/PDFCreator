using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class RecommendPdfArchitectWindow : Window
    {
        public RecommendPdfArchitectWindow(RecommendPdfArchitectWindowViewModel viewModel, ITranslator translator)
        {
            DataContext = viewModel;

            InitializeComponent();

            translator.Translate(this);
        }
    }
}