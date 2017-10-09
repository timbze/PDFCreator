using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp
{
    public class EditEmailTextViewModel : OverlayViewModelBase<EditEmailTextInteraction, SmtpTranslation>
    {
        private readonly string _signatureText;
        public TokenViewModel SubjectTokenViewModel { get; set; }
        public TokenViewModel ContentTokenViewModel { get; set; }

        private readonly TokenReplacer _tokenReplacer;

        public EditEmailTextViewModel(ITranslationUpdater translationUpdater, IMailSignatureHelper mailSignatureHelper, TokenHelper tokenHelper)
            : base(translationUpdater)
        {
            if (tokenHelper != null)
            {
                _tokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

                SubjectTokenViewModel = new TokenViewModel(x => Interaction.Subject = x, () => Interaction?.Subject, tokenHelper.GetTokenListForEmail(), ReplaceTokens);
                ContentTokenViewModel = new TokenViewModel(x => Interaction.Content = x, () => Interaction?.Content, tokenHelper.GetTokenListForEmail(), ReplaceTokensAddSignature);
            }

            _signatureText = mailSignatureHelper.ComposeMailSignature();

            OkCommand = new DelegateCommand(ExecuteOk);
            CancelCommand = new DelegateCommand(o => FinishInteraction());
        }

        private string ReplaceTokens(string s)
        {
            if (s == null)
                return string.Empty;

            return _tokenReplacer.ReplaceTokens(s);
        }

        private string ReplaceTokensAddSignature(string s)
        {
            if (s == null)
                return string.Empty;

            var contentString = _tokenReplacer.ReplaceTokens(s);

            if (AddSignature)
                contentString += _signatureText;

            return contentString;
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private void ExecuteOk(object o)
        {
            Interaction.Success = true;

            FinishInteraction();
        }

        public bool AddSignature
        {
            get { return Interaction?.AddSignature == true; }
            set
            {
                Interaction.AddSignature = value;
                RaisePropertyChanged(nameof(AddSignature));
                ContentTokenViewModel.RaiseTextChanged();
            }
        }

        protected override void HandleInteractionObjectChanged()
        {
            RaisePropertyChanged(nameof(ContentTokenViewModel));
            RaisePropertyChanged(nameof(SubjectTokenViewModel));
            RaisePropertyChanged(nameof(AddSignature));
            RaisePropertyChanged(nameof(Interaction));
            ContentTokenViewModel?.RaiseTextChanged();
        }

        public override string Title => Translation.EditMailText;
    }
}
