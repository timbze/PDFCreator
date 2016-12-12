using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class FtpActionViewModel : ActionViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public FtpActionViewModel(ITranslator translator, IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
            Translator = translator;

            SetPasswordCommand = new DelegateCommand(SetPasswordExecute);

            DisplayName = Translator.GetTranslation("FtpActionSettings", "DisplayName");
            Description = Translator.GetTranslation("FtpActionSettings", "Description");

            var tokenHelper = new TokenHelper(Translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            TokenViewModel = new TokenViewModel(x => CurrentProfile.Ftp.Directory = x, () => CurrentProfile?.Ftp.Directory, TokenReplacer.GetTokenNames(true));
        }

        public TokenViewModel TokenViewModel { get; set; }

        public ITranslator Translator { get; }

        public TokenReplacer TokenReplacer { get; set; }

        private string Password
        {
            get { return CurrentProfile.Ftp.Password; }
            set { CurrentProfile.Ftp.Password = value; }
        }

        public DelegateCommand SetPasswordCommand { get; set; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.Ftp.Enabled; }
            set
            {
                CurrentProfile.Ftp.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.UploadWithFtp;
        }

        protected override void HandleCurrentProfileChanged()
        {
            TokenViewModel.RaiseTextChanged();
        }

        private void SetPasswordExecute(object obj)
        {
            var title = Translator.GetTranslation("FtpActionSettings", "PasswordTitle");
            var passwordDescription = Translator.GetTranslation("FtpActionSettings", "PasswordDescription");

            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, title, passwordDescription);
            interaction.Password = Password;

            _interactionInvoker.Invoke(interaction);

            if (interaction.Result == PasswordResult.StorePassword)
            {
                Password = interaction.Password;
            }
            else if (interaction.Result == PasswordResult.RemovePassword)
            {
                Password = "";
            }
        }
    }
}