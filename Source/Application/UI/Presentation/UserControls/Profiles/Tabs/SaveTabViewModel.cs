using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
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
        private readonly ITokenViewModelFactory _tokenViewModelFactory;

        public string Title => Translation.SaveTabText;
        public IconList Icon => IconList.SaveSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;

        private TokenReplacer _tokenReplacer = new TokenReplacer();
        public TokenViewModel<ConversionProfile> FileNameViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> FolderViewModel { get; private set; }
        public bool AllowSkipPrintDialog { get; }

        public SaveTabViewModel(TokenButtonFunctionProvider buttonFunctionProvider, ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater, EditionHintOptionProvider editionHintOptionProvider, TokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory)
            : base(translationUpdater, selectedProfileProvider)
        {
            AllowSkipPrintDialog = !editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            _buttonFunctionProvider = buttonFunctionProvider;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels());
        }

        private void SetTokenViewModels()
        {
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
            var builder = _tokenViewModelFactory
                .BuilderWithSelectedProfile();

            FileNameViewModel = builder
                .WithTokenList(_tokenHelper.GetTokenListForFilename())
                .WithTokenCustomPreview(PreviewForFileName)
                .WithSelector(p => p.FileNameTemplate)
                .Build();

            FolderViewModel = builder
                    .WithTokenList(_tokenHelper.GetTokenListForDirectory())
                    .WithTokenCustomPreview(PreviewForFolder)
                    .WithSelector(p => p.TargetDirectory)
                    .WithButtonCommand(_buttonFunctionProvider.GetBrowseFolderFunction(Translation.ChooseFolder))
                    .Build();

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
