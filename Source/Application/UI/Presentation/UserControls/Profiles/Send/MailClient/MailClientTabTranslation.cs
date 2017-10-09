using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public class MailClientTabTranslation : ITranslatable
    {
        public string CheckMailClient { get; private set; } = "Check e-mail client";
        public string NoMapiClientFound { get; private set; } = "Could not find MAPI client (e.g. Thunderbird or Outlook).";
        public string CheckMailClientButtonText { get; private set; } = "Check E-Mail Client";
        public string EditEmailButtonText { get; private set; } = "Edit E-Mail Text";
        public string MultipleRecipientsHintText { get; private set; } = "Multiple recipients are separated by commas";
        public string RecipientsText { get; private set; } = "Recipients:";
        public string Email { get; set; } = "Email";
    }
}
