namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class EditionHintOptionProvider
    {
        public EditionHintOptionProvider(bool showOnlyForPlusAndBusinessHint)
        {
            ShowOnlyForPlusAndBusinessHint = showOnlyForPlusAndBusinessHint;
        }

        public bool ShowOnlyForPlusAndBusinessHint { get; }
    }
}
