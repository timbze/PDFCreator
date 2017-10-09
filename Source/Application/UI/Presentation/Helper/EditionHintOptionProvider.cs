namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class EditionHintOptionProvider
    {
        public EditionHintOptionProvider(bool showOnlyForPlusAndBusinessHint, bool showOnlyForBusinessHint)
        {
            ShowOnlyForPlusAndBusinessHint = showOnlyForPlusAndBusinessHint;
            ShowOnlyForBusinessHint = showOnlyForBusinessHint;
        }

        public bool ShowOnlyForPlusAndBusinessHint { get; }
        public bool ShowOnlyForBusinessHint { get; }
    }
}
