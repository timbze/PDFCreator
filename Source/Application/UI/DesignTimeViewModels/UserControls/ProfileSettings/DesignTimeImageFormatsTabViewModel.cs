using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeImageFormatsTabViewModel : ImageFormatsTabViewModel
    {
        public DesignTimeImageFormatsTabViewModel() : base(new TranslationProxy())
        {
        }
    }
}