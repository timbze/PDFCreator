namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class PlusHintHelperDisabled : IPlusHintHelper
    {
        public int CurrentJobCounter => 0;
        public bool QueryDisplayHint()
        {
            return false;
        }
    }
}