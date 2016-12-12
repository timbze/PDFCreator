using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimeAutosaveTabViewModel : AutoSaveTabViewModel
    {
        public DesignTimeAutosaveTabViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker())
        {
        }
    }
}