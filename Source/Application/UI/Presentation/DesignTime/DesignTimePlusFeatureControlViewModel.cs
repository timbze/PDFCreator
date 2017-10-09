using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimePlusFeatureControlViewModel : PlusFeatureControlViewModel
    {
        public DesignTimePlusFeatureControlViewModel() : base(new EditionHintOptionProvider(true, true), new ProcessStarter(), new DesignTimeTranslationUpdater())
        {
        }
    }
}
