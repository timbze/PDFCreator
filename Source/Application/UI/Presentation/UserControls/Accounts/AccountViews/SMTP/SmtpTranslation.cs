using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class SmtpTranslation : AccountsTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        //ProfileSubTab
        public string SmtpSubTabTitle { get; private set; } = "SMTP";

        //SmtpPasswordStep
        public string SmtpPasswordOverlayTitle { get; private set; } = "SMTP Mail";

        public string SmtpAccountLabel { get; private set; } = "SMTP Account:";
        public string SmtpServerPasswordLabel { get; private set; } = "SMTP Server Password:";

        //Edit Command
        public string EditSmtpAccount { get; private set; } = "Edit SMTP Account";

        //Remove Command
        public string RemoveSmtpAccount { get; private set; } = "Remove SMTP Account";

        private string[] SmtpGetsDisabled { get; set; } = { "The SMTP action will be disabled for this profile.", "The SMTP action will be disabled for this profiles." };

        public string GetSmtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, SmtpGetsDisabled);
        }

        //AccountView
        public string EmailAddressLabel { get; private set; } = "E-Mail Address:";

        public string PortLabel { get; private set; } = "Port:";
        public string Ssl { get; private set; } = "SSL";

        //ActionUserControl
        public string SendEmailViaSmtp { get; private set; } = "Send e-mail via SMTP";

        public string SelectSmtpAccountLabel { get; private set; } = "Please select a SMTP account:";
        public string RecipientsLabel { get; private set; } = "Recipients:";
        public string MultipleRecipientsSeperatedByCommas { get; private set; } = "Multiple recipients are separated by commas";
        public string EditMailText { get; private set; } = "Edit E-Mail Text";
        public string SendTestMail { get; private set; } = "Send Test Mail";

        //TestAssistant
        public string AttachmentFile { get; private set; } = "AttachmentFile";

        private string TestMailSent { get; set; } = "Test mail sent to {0}.";
        public string SetSmtpServerPassword { get; private set; } = "Set SMTP Server Password";
        public string NoAccount { get; private set; } = "The specified SMTP account is not configured.";

        public string GetTestMailSentFormattedTranslation(string reciepent)
        {
            return string.Format(TestMailSent, reciepent);
        }

        public string AttachSignatureText { get; private set; } = "Attach pdfforge signature";
        public string BodyTextLabel { get; private set; } = "_Text:";
        public string SubjectLabel { get; private set; } = "_Subject:";
        public string UseHtml { get; private set; } = "Use Html";
    }
}
