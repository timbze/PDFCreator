using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class EmailClientActionSettingsAndActionTranslation : ITranslatable
    {
        public string CheckMailClient { get; private set; } = "Check e-mail client";
        public string Description { get; private set; } = "Opens a new e-mail in the default client. There you can add recipients, text and other information and then send the mail to your contacts.";
        public string DisplayName { get; private set; } = "Open e-mail client";
        public string NoMapiClientFound { get; private set; } = "Could not find MAPI client (e.g. Thunderbird or Outlook).";
        public string CheckMailClientButtonText { get; private set; } = "Check E-Mail Client";
        public string EditEmailButtonText { get; private set; } = "Edit E-Mail Text";
        public string MultipleRecipientsHintText { get; private set; } = "Multiple recipients are separated by commas";
        public string RecipientsText { get; private set; } = "Recipients:";
    }
}
