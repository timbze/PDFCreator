namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public interface IPlusHintHelper
    {
        int CurrentJobCounter { get; }
        bool QueryDisplayHint();
    }
}