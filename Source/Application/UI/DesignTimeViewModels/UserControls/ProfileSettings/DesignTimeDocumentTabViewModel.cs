using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeDocumentTabViewModel : DocumentTabViewModel
    {
        public DesignTimeDocumentTabViewModel() : base(new DocumentTabTranslation(), null, null, new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
        }
    }
}