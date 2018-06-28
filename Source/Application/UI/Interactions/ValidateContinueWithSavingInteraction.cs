using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class ValidateContinueWithSavingInteraction : MessageInteraction
    {
        public ValidateContinueWithSavingInteraction(string descriptionText, string title) : base(descriptionText, title, MessageOptions.YesNoCancel, MessageIcon.Question)
        {
        }
    }
}
