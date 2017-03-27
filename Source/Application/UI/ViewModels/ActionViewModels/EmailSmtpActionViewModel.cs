using System.Text;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class EmailSmtpActionViewModel : ActionViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ISmtpTest _smtpTest;

        public EmailSmtpActionViewModel(SmtpSettingsAndActionControlTranslation translation, IInteractionInvoker interactionInvoker, ISmtpTest smtpTest)
        {
            Translation = translation;
            _interactionInvoker = interactionInvoker;
            _smtpTest = smtpTest;

            EditMailTextCommand = new DelegateCommand(EditMailTextExecute);
            SetPasswordCommand = new DelegateCommand(SetPasswordExecute);
            TestSmtpCommand = new DelegateCommand(TextSmtpExecute);

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
        }

        public SmtpSettingsAndActionControlTranslation Translation { get; }

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
            sb.AppendLine(Translation.RecipientsText);
            sb.AppendLine(EmailSmtpSettings.Recipients);

            var title = Translation.SmtpPasswordTitle;
            var description = Translation.SmtpPasswordDescription;

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
            var interaction = new EditEmailTextInteraction(EmailSmtpSettings.Subject, EmailSmtpSettings.Content, EmailSmtpSettings.AddSignature, EmailSmtpSettings.Html);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            EmailSmtpSettings.AddSignature = interaction.AddSignature;
            EmailSmtpSettings.Content = interaction.Content;
            EmailSmtpSettings.Subject = interaction.Subject;
            EmailSmtpSettings.Html = interaction.Html;
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.SendEmailOverSmtp;
        }
    }
}