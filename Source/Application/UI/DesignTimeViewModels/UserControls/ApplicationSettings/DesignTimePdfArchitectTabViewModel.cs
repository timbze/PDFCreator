using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimePdfArchitectTabViewModel : PdfArchitectTabViewModel
    {
        public DesignTimePdfArchitectTabViewModel() : base(null, null, new PdfArchitectTabTranslation())
        {
        }
    }
}