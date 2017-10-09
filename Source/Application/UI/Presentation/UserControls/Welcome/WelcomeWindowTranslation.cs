using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeWindowTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Welcome";
        public string WelcomeText { get; private set; } = "The improvements of this version are listed in the \"What's new?\" section of the manual. For latest information about our products, join us on facebook or google+.";
        public string WelcomeTextHeadlineText { get; private set; } = "Thank you for downloading PDFCreator.";
        public string WhatsNewButtonContent { get; private set; } = "What's new?";
    }
}
