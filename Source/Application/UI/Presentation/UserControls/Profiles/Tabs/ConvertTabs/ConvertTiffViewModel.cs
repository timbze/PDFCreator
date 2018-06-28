using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertTiffViewModel : ProfileUserControlViewModel<ConvertTiffTranslation>
    {
        public ConvertTiffViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
        }
    }
}
