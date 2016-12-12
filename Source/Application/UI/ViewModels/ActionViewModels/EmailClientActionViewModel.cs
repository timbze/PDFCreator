using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class EmailClientActionViewModel : ActionViewModel
    {
        private readonly IClientTestEmail _clientTestEmail;
        private readonly IInteractionInvoker _interactionInvoker;

        public EmailClientActionViewModel(ITranslator translator, IInteractionInvoker interactionInvoker, IClientTestEmail clientTestEmail)
        {
            _interactionInvoker = interactionInvoker;
            _clientTestEmail = clientTestEmail;
            Translator = translator;

            EmailClientTestCommand = new DelegateCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            DisplayName = Translator.GetTranslation("EmailClientActionSettings", "DisplayName");
            Description = Translator.GetTranslation("EmailClientActionSettings", "Description");
        }

        public ITranslator Translator { get; }

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
            var caption = Translator.GetTranslation("EmailClientActionSettings", "CheckMailClient");
            var message = Translator.GetTranslation("EmailClientActionSettings", "NoMapiClientFound");

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);
        }

        private void EditEmailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailClientSettings.Subject, EmailClientSettings.Content, EmailClientSettings.AddSignature);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            EmailClientSettings.AddSignature = interaction.AddSignature;
            EmailClientSettings.Content = interaction.Content;
            EmailClientSettings.Subject = interaction.Subject;
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.OpenEmailClient;
        }
    }
}