using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHomeViewModel : HomeViewModel
    {
        public DesignTimeHomeViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimePrinterHelper(), new DesignTimePrintJobViewModel.DesignTimeSettingsProvider())
        {
        }
    }
}
