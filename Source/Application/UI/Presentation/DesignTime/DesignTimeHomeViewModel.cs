using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHomeViewModel : HomeViewModel
    {
        public DesignTimeHomeViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimePrinterHelper(), new DesignTimePrintJobViewModel.DesignTimeSettingsProvider(), new DesignTimeJobHistoryManager(), new DispatcherWrapper(), new DesignTimeCommandLocator())
        {
        }
    }
}
