using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertPngViewModel : ProfileUserControlViewModel<ConvertPngTranslation>
    {
        public ConvertPngViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
        }
    }
}
