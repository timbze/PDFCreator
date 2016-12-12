using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeTitleTabViewModel : TitleTabViewModel
    {
        public DesignTimeTitleTabViewModel() : base(new TranslationProxy())
        {
        }
    }
}