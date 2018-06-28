using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeWindowTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Welcome";
        public string WelcomeText { get; private set; } = "The improvements of this version are listed in the \"What's new?\" section of the manual.";
        public string WelcomeTextHeadlineText { get; private set; } = "Thank you for downloading PDFCreator.";
        public string WhatsNewButtonContent { get; private set; } = "What's new?";
        public string PrioritySupport { get; private set; } = "Priority Support";
        public string Blog { get; private set; } = "Blog";
    }
}
