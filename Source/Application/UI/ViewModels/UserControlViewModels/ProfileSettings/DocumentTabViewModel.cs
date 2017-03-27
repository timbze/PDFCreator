using System.Drawing;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class DocumentTabViewModel : CurrentProfileViewModel
    {
        private readonly IFontHelper _fontHelper;
        private readonly IInteractionInvoker _interactionInvoker;

        public DocumentTabViewModel(DocumentTabTranslation translation, IInteractionInvoker interactionInvoker, IFontHelper fontHelper, TokenHelper tokenHelper)
        {
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;
            Translation = translation;

           TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            TitleViewModel = new TokenViewModel(
                s => CurrentProfile.TitleTemplate = s,
                () => CurrentProfile?.TitleTemplate,
                tokenHelper.GetTokenListForTitle());

            AuthorViewModel = new TokenViewModel(
                s => CurrentProfile.AuthorTemplate = s,
                () => CurrentProfile?.AuthorTemplate,
                tokenHelper.GetTokenListForAuthor());

            SubjectViewModel = new TokenViewModel(s => CurrentProfile.SubjectTemplate = s,() => CurrentProfile?.SubjectTemplate, tokenHelper.GetTokenListForSubjectAndKeyword());
            KeywordViewModel = new TokenViewModel(s => CurrentProfile.KeywordTemplate = s,() => CurrentProfile?.KeywordTemplate, tokenHelper.GetTokenListForSubjectAndKeyword());

        }

        public TokenViewModel AuthorViewModel { get; set; }

        public TokenViewModel TitleViewModel { get; set; }

        public TokenViewModel SubjectViewModel { get; set; }

        public TokenViewModel KeywordViewModel { get; set; }
        
        public TokenReplacer TokenReplacer { get; set; }

        public DocumentTabTranslation Translation { get; set; }

        public string FontButtonText { get; set; } = "Select Font";

        public DelegateCommand ChooseStampFont => new DelegateCommand(ChooseStampFontExecute);
        public DelegateCommand ChooseStampColor => new DelegateCommand(ChooseStampColorExecute);

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.ProfileDocument;
        }

        protected override void HandleCurrentProfileChanged()
        {
            UpdateFontButtonText(CurrentProfile.Stamping);
            AuthorViewModel.RaiseTextChanged();
            TitleViewModel.RaiseTextChanged();
            SubjectViewModel.RaiseTextChanged();
            KeywordViewModel.RaiseTextChanged();
        }

        private void ChooseStampColorExecute(object obj)
        {
            var interaction = new ColorInteraction();
            interaction.Color = CurrentProfile.Stamping.Color;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            CurrentProfile.Stamping.Color = interaction.Color;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void ChooseStampFontExecute(object obj)
        {
            var interaction = new FontInteraction();
            interaction.Font = new Font(CurrentProfile.Stamping.FontName, CurrentProfile.Stamping.FontSize);

            _interactionInvoker.Invoke(interaction);
            if (!interaction.Success)
                return;

            var postScriptName = _fontHelper.FindPostScriptName(interaction.Font.Name);

            if (postScriptName == null)
            {
                DisplayFontError();
                return;
            }

            CurrentProfile.Stamping.FontName = interaction.Font.Name;
            CurrentProfile.Stamping.PostScriptFontName = postScriptName;
            CurrentProfile.Stamping.FontSize = interaction.Font.Size;

            UpdateFontButtonText(CurrentProfile.Stamping);
        }

        private void UpdateFontButtonText(Stamping stamping)
        {
            var fontSize = stamping.FontSize.ToString();
            var fontstring = $"{stamping.FontName} {fontSize}pt";
            if (fontstring.Length > 25)
            {
                fontstring = stamping.FontName.Substring(0, 25 - 4 - fontSize.Length).TrimEnd();
                fontstring = $"{fontstring}. {fontSize}pt";
            }

            FontButtonText = fontstring;
            RaisePropertyChanged(nameof(FontButtonText));
        }

        private void DisplayFontError()
        {
            var message = Translation.FontFileNotSupported;

            var interaction = new MessageInteraction(message, "PDFCreator", MessageOptions.OK, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);
        }
    }
}