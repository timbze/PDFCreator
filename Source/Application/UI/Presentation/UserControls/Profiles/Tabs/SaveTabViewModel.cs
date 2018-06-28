using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class SaveTabViewModel : ProfileUserControlViewModel<SaveTabTranslation>, ITabViewModel
    {
        private readonly TokenButtonFunctionProvider _buttonFunctionProvider;
        private readonly TokenHelper _tokenHelper;

        public string Title => Translation.SaveTabText;
        public IconList Icon => IconList.SaveSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;

        private TokenReplacer _tokenReplacer = new TokenReplacer();
        public TokenViewModel FileNameViewModel { get; private set; }
        public TokenViewModel FolderViewModel { get; private set; }
        public bool AllowSkipPrintDialog { get; }

        public SaveTabViewModel(TokenButtonFunctionProvider buttonFunctionProvider, ISelectedProfileProvider selectedProfileProvider, ITranslationUpdater translationUpdater, EditionHintOptionProvider editionHintOptionProvider, TokenHelper tokenHelper)
            : base(translationUpdater, selectedProfileProvider)
        {
            AllowSkipPrintDialog = !editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            _buttonFunctionProvider = buttonFunctionProvider;
            _tokenHelper = tokenHelper;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels());
            selectedProfileProvider.SelectedProfileChanged += (sender, args) => SetTokenViewModels();
        }

        private void SetTokenViewModels()
        {
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            FileNameViewModel = new TokenViewModel(
                s => CurrentProfile.FileNameTemplate = s,
                () => CurrentProfile?.FileNameTemplate,
                _tokenHelper.GetTokenListForFilename(),
                PreviewForFileName);

            FolderViewModel = new TokenViewModel(
                s => CurrentProfile.TargetDirectory = s,
                () => CurrentProfile?.TargetDirectory,
                _tokenHelper.GetTokenListForDirectory(),
                PreviewForFolder,
                _buttonFunctionProvider.GetBrowseFolderFunction(Translation.ChooseFolder));

            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
        }

        private string PreviewForFileName(string s)
        {
            return ValidName.MakeValidFileName(_tokenReplacer.ReplaceTokens(s));
        }

        private string PreviewForFolder(string s)
        {
            return ValidName.MakeValidFolderName(_tokenReplacer.ReplaceTokens(s));
        }
    }
}
