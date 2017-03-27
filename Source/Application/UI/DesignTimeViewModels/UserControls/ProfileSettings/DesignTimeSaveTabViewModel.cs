using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeSaveTabViewModel : SaveTabViewModel
    {
        public DesignTimeSaveTabViewModel() : base(new SaveTabTranslation(), new DesignTimeInteractionInvoker(), new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
        }
    }
}