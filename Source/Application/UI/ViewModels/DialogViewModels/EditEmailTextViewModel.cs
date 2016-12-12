using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class EditEmailTextViewModel : InteractionAwareViewModelBase<EditEmailTextInteraction>
    {
        private readonly string _signatureText;

        public EditEmailTextViewModel(ITranslator translator)
        {
            Translator = translator;
            var tokenHelper = new TokenHelper(translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            var tokens = tokenHelper.GetTokenListForEmail();

            var mailSignatureHelper = new MailSignatureHelper(translator);
            _signatureText = mailSignatureHelper.ComposeMailSignature();

            SubjectTextViewModel = new TokenViewModel(
                s => Interaction.Subject = s,
                () => Interaction?.Subject,
                tokens);

            BodyTextViewModel = new TokenViewModel(
                s => Interaction.Content = s,
                () => Interaction?.Content,
                tokens);

            OkCommand = new DelegateCommand(ExecuteOk);
        }

        public TokenReplacer TokenReplacer { get; set; }

        public ITranslator Translator { get; }

        public TokenViewModel BodyTextViewModel { get; set; }

        public TokenViewModel SubjectTextViewModel { get; set; }

        public ICommand OkCommand { get; }

        public bool AddSignature
        {
            get { return Interaction?.AddSignature == true; }
            set
            {
                Interaction.AddSignature = value;
                RaisePropertyChanged(nameof(AddSignature));
                RaisePropertyChanged(nameof(Footer));
            }
        }

        public string Footer => AddSignature ? _signatureText : "";

        protected override void HandleInteractionObjectChanged()
        {
            RaisePropertyChanged(nameof(BodyTextViewModel));
            RaisePropertyChanged(nameof(SubjectTextViewModel));
            RaisePropertyChanged(nameof(AddSignature));
            RaisePropertyChanged(nameof(Footer));
        }

        private void ExecuteOk(object o)
        {
            Interaction.AddSignature = AddSignature;
            Interaction.Success = true;

            FinishInteraction();
        }
    }
}