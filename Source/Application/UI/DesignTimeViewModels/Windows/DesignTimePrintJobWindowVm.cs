using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimePrintJobWindowVm : PrintJobViewModel
    {
        public DesignTimePrintJobWindowVm() : base(new DesignTimeSettingsManager(), new DesignTimeJobInfoQueue(), new TranslationProxy(), new DesignTimeDragAndDropHandler(), new DesignTimeInteractionInvoker(), new DesignTimeUserGuideHelper(), new ApplicationNameProvider("PDFCreator with Edition Name"))
        {
        }
    }
}