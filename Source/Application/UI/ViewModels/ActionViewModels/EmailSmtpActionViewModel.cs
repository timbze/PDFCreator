using System.Text;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class EmailSmtpActionViewModel : ActionViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ISmtpTest _smtpTest;

        public EmailSmtpActionViewModel(ITranslator translator, IInteractionInvoker interactionInvoker, ISmtpTest smtpTest)
        {
            Translator = translator;
            _interactionInvoker = interactionInvoker;
            _smtpTest = smtpTest;

            EditMailTextCommand = new DelegateCommand(EditMailTextExecute);
            SetPasswordCommand = new DelegateCommand(SetPasswordExecute);
            TestSmtpCommand = new DelegateCommand(TextSmtpExecute);

            DisplayName = Translator.GetTranslation("SmtpEmailActionSettings", "DisplayName");
            Description = Translator.GetTranslation("SmtpEmailActionSettings", "Description");
        }

        public ITranslator Translator { get; }

        // this property is here so the client checkbox can be disabled in PDFCreator Server
        public bool DisplayMailClientTextCheckbox { get; set; } = true;

        public DelegateCommand EditMailTextCommand { get; set; }
        public DelegateCommand SetPasswordCommand { get; set; }
        public DelegateCommand TestSmtpCommand { get; set; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.EmailSmtpSettings.Enabled; }
            set
            {
                CurrentProfile.EmailSmtpSettings.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private EmailSmtpSettings EmailSmtpSettings => CurrentProfile.EmailSmtpSettings;

        private void SetPasswordExecute(object obj)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Translator.GetTranslation("pdfforge.PDFCreator.UI.Views.ActionControls.EmailClientActionControl", "RecipientsText.Text"));
            sb.AppendLine(EmailSmtpSettings.Recipients);

            var title = Translator.GetTranslation("EmailClientActionSettings", "SmtpPasswordTitle");
            var description = Translator.GetTranslation("EmailClientActionSettings", "SmtpPasswordDescription");

            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, title, description);
            interaction.Password = EmailSmtpSettings.Password;
            interaction.IntroText = sb.ToString();

            _interactionInvoker.Invoke(interaction);

            if (interaction.Result == PasswordResult.StorePassword)
            {
                EmailSmtpSettings.Password = interaction.Password;
            }
            else if (interaction.Result == PasswordResult.StorePassword)
            {
                EmailSmtpSettings.Password = "";
            }
        }

        private void TextSmtpExecute(object obj)
        {
            _smtpTest.SendTestMail(CurrentProfile, Accounts);
        }

        private void EditMailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailSmtpSettings.Subject, EmailSmtpSettings.Content, EmailSmtpSettings.AddSignature);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            EmailSmtpSettings.AddSignature = interaction.AddSignature;
            EmailSmtpSettings.Content = interaction.Content;
            EmailSmtpSettings.Subject = interaction.Subject;
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.SendEmailOverSmtp;
        }
    }
}