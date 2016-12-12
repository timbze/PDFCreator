using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeActionsTabViewModel : ActionsTabViewModel
    {
        public DesignTimeActionsTabViewModel() : base(new TranslationProxy())
        {
        }
    }
}