using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    internal class DesignTimePlusHintHelper : IPlusHintHelper
    {
        public int CurrentJobCounter => 42;

        public bool QueryDisplayHint()
        {
            return false;
        }
    }
}