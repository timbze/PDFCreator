using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint
{
    public class PlusHintViewTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string HeadlineText { get; private set; } = "We highly value that you are using PDFCreator!";
        public string NoThanksButtonContent { get; private set; } = "No, thanks";
        public string PlusButtonContent { get; private set; } = "More Information";
        public string RecommendPlusText { get; private set; } = "For power users like you, we recommend PDFCreator Plus to support the future development and benefit from exclusive features and priority support.";
        private string[] ThankYou { get; set; } = { "You have already converted {0} file!", "You have already converted {0} files!" };
        public string Title { get; private set; } = "Try PDFCreator Plus!";

        public string GetThankYouMessage(int numberOfPrintJobs)
        {
            return PluralBuilder.GetFormattedPlural(numberOfPrintJobs, ThankYou);
        }
    }
}
