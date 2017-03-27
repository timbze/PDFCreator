using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class SmtpSettingsAndActionControlTranslation : ITranslatable
    {

        public string Description { get; private set; } = "The SMTP e-mail action allows to directly send files via e-mail without further user interaction. Notice: This action is intended for advanced users and requires careful attention as it can silently send the converted documents via e-mail to the configured recipients.";
        public string DisplayName { get; private set; } = "Send e-mail over SMTP";
        public string SendTestMail { get; private set; } = "Send test mail";
        private string TestMailSent { get; set; } = "Test mail sent to {0}.";
        public string SmtpPasswordDescription { get; private set; } = "Smtp Password:";
        public string SmtpPasswordTitle { get; private set; } = "Set Smtp Password";
        public string RecipientsText { get; private set; } = "Recipients:";
        public string EditMailButtonText { get; private set; } = "Edit E-Mail Text";
        public string EmailAddressText { get; private set; } = "E-Mail Address:";
        public string MultipleRecipientsHintText { get; private set; } = "Multiple recipients are separated by commas";
        public string PortText { get; private set; } = "Port:";
        public string SendTestMailButtonText { get; private set; } = "Send Test Mail";
        public string ServerText { get; private set; } = "Server:";
        public string SetPasswordText { get; private set; } = "Set Password";
        public string SslText { get; private set; } = "SSL";
        public string UsernameText { get; private set; } = "User Name:";
        public string RetypeSmtpPwMessage { get; private set; } = "Could not authenticate at server.\r\nPlease check your password and verify that you have a working internet connection.";

        public string GetTestMailSentFormattedTranslation(string reciepent)
        {
            return string.Format(TestMailSent, reciepent);
        }
    }
}
