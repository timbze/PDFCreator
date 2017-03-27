using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class EmailClientActionViewModel : ActionViewModel
    {
        private readonly IClientTestEmail _clientTestEmail;
        private readonly IInteractionInvoker _interactionInvoker;

        public EmailClientActionViewModel(EmailClientActionSettingsAndActionTranslation translation, IInteractionInvoker interactionInvoker, IClientTestEmail clientTestEmail)
        {
            _interactionInvoker = interactionInvoker;
            _clientTestEmail = clientTestEmail;
            Translation = translation;

            EmailClientTestCommand = new DelegateCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
        }

        public EmailClientActionSettingsAndActionTranslation Translation { get; }

        public DelegateCommand EmailClientTestCommand { get; set; }
        public DelegateCommand EditEmailTextCommand { get; set; }

        private EmailClientSettings EmailClientSettings => CurrentProfile.EmailClientSettings;

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.EmailClientSettings.Enabled; }
            set
            {
                CurrentProfile.EmailClientSettings.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private void EmailClientTestExecute(object obj)
        {
            var success = _clientTestEmail.SendTestEmail(EmailClientSettings);
            if (!success)
                DisplayNoClientFoundMessage();
        }

        private void DisplayNoClientFoundMessage()
        {
            var caption = Translation.CheckMailClient;
            var message = Translation.NoMapiClientFound;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);
        }

        private void EditEmailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailClientSettings.Subject, EmailClientSettings.Content, EmailClientSettings.AddSignature, EmailClientSettings.Html);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            EmailClientSettings.AddSignature = interaction.AddSignature;
            EmailClientSettings.Content = interaction.Content;
            EmailClientSettings.Subject = interaction.Subject;
            EmailClientSettings.Html = interaction.Html;
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.OpenEmailClient;
        }
    }
}