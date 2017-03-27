using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class EditEmailTextViewModel : InteractionAwareViewModelBase<EditEmailTextInteraction>
    {
        private readonly string _signatureText;

        public EditEmailTextViewModel(EditEmailTextWindowTranslation translation, IMailSignatureHelper mailSignatureHelper, TokenHelper tokenHelper)
        {
            Translation = translation;
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            var tokens = tokenHelper.GetTokenListForEmail();

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

        public EditEmailTextWindowTranslation Translation { get; }

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