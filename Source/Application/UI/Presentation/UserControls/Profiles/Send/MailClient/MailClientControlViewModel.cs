using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public class MailClientControlViewModel : ProfileUserControlViewModel<MailClientTabTranslation>
    {
        private readonly IClientTestEmail _clientTestEmail;
        private readonly IInteractionRequest _interactionRequest;
        public TokenViewModel RecipientsTokenViewModel { get; private set; }
        public DelegateCommand EmailClientTestCommand { get; set; }
        public DelegateCommand EditEmailTextCommand { get; set; }
        private EmailClientSettings EmailClientSettings => CurrentProfile?.EmailClientSettings;

        public MailClientControlViewModel(IInteractionRequest interactionRequest, IClientTestEmail clientTestEmail, ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider, TokenHelper tokenHelper) : base(translationUpdater, selectedProfileProvider)
        {
            _interactionRequest = interactionRequest;
            _clientTestEmail = clientTestEmail;

            var tokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            var tokens = tokenHelper.GetTokenListForEmailRecipients();
            if (CurrentProfile?.EmailClientSettings != null)
                RecipientsTokenViewModel = new TokenViewModel(
                    v => EmailClientSettings.Recipients = v,
                    () => EmailClientSettings.Recipients,
                    tokens,
                    s => tokenReplacer.ReplaceTokens(s));
            RaisePropertyChanged(nameof(RecipientsTokenViewModel));

            EmailClientTestCommand = new DelegateCommand(EmailClientTestExecute);
            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);
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
            _interactionRequest.Raise(interaction);
        }

        private void EditEmailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailClientSettings.Subject, EmailClientSettings.Content, EmailClientSettings.AddSignature, EmailClientSettings.Html);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailClientSettings.AddSignature = interaction.AddSignature;
            EmailClientSettings.Content = interaction.Content;
            EmailClientSettings.Subject = interaction.Subject;
            EmailClientSettings.Html = interaction.Html;
        }
    }
}
