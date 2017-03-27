using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeAutosaveTabViewModel : AutoSaveTabViewModel
    {
        public DesignTimeAutosaveTabViewModel() : base(new DesignTimeInteractionInvoker(), new AutosaveTabTranslation(), new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
        }
    }
}