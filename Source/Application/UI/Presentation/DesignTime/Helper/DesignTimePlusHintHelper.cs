using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimePlusHintHelper : IPlusHintHelper
    {
        public int CurrentJobCounter => 101;

        public bool QueryDisplayHint()
        {
            return true;
        }
    }
}
