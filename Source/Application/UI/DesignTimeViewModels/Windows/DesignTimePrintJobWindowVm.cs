using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimePrintJobWindowVm : PrintJobViewModel
    {
        public DesignTimePrintJobWindowVm() : base(new DesignTimeSettingsManager(), new DesignTimeJobInfoQueue(), new PrintJobViewModelTranslation(), new DesignTimeDragAndDropHandler(), new DesignTimeInteractionInvoker(), new DesignTimeUserGuideHelper(), new ApplicationNameProvider("PDFCreator with Edition Name"), new DispatcherWrapper())
        {
        }
    }
}