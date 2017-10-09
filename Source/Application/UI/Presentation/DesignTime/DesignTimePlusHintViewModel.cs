using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePlusHintViewModel : PlusHintViewModel
    {
        public DesignTimePlusHintViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator(), new DesignTimePlusHintHelper())
        {
        }
    }
}
