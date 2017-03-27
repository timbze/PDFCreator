using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeRecommendPdfArchitectWindowViewModel : RecommendPdfArchitectWindowViewModel
    {
        public DesignTimeRecommendPdfArchitectWindowViewModel() : base(null, null, new RecommendPdfArchitectWindowTranslation())
        {
        }
    }
}