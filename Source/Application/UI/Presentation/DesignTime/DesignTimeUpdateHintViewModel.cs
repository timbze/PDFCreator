using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateHintViewModel : UpdateHintViewModel
    {
        public DesignTimeUpdateHintViewModel() : base(new DisabledUpdateAssistant(), new ProcessStarter(), new DesignTimeTranslationUpdater(), new EventAggregator(), new DesignTimeVersionHelper())
        {
        }
    }
}
