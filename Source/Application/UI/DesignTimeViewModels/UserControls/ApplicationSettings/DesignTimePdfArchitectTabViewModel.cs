using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimePdfArchitectTabViewModel : PdfArchitectTabViewModel
    {
        public DesignTimePdfArchitectTabViewModel() : base(new TranslationProxy(), null, null)
        {
        }
    }
}