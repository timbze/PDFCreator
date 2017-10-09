namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IPlusHintHelper
    {
        int CurrentJobCounter { get; }

        bool QueryDisplayHint();
    }
}
