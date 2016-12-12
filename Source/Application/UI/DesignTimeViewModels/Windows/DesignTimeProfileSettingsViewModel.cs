using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeProfileSettingsViewModel : ProfileSettingsViewModel
    {
        public DesignTimeProfileSettingsViewModel()
            : base(new DesignTimeInteractionInvoker(),
                new TranslationProxy(),
                null,
                new ProfileSettingsViewModelBundle(
                    new DesignTimeDocumentTabViewModel(),
                    new DesignTimeSaveTabViewModel(),
                    new DesignTimeAutosaveTabViewModel(),
                    new DesignTimeActionsTabViewModel(),
                    new DesignTimeImageFormatsTabViewModel(),
                    new DesignTimePdfTabViewModel()))
        {
        }
    }
}