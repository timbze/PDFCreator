using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeImageFormatsTabViewModel : ImageFormatsTabViewModel
    {
        public DesignTimeImageFormatsTabViewModel() : base(new ImageFormatsTabTranslation())
        {
        }
    }
}