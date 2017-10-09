using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Tokens;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class MetadataViewModel : TranslatableViewModelBase<MetadataTabTranslation>, ITabViewModel
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private TokenReplacer _tokenReplacer = new TokenReplacer();
        private readonly TokenHelper _tokenHelper;

        public string Title => Translation.Title;
        public IconList Icon => IconList.MetadataSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;

        public TokenViewModel TitleTokenViewModel { get; private set; }
        public TokenViewModel AuthorTokenViewModel { get; private set; }
        public TokenViewModel SubjectTokenViewModel { get; private set; }
        public TokenViewModel KeywordsTokenViewModel { get; private set; }

        public MetadataViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, TokenHelper tokenHelper) : base(translationUpdater)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _tokenHelper = tokenHelper;

            translationUpdater.RegisterAndSetTranslation(SetTokenViewModels);
            _currentSettingsProvider.SelectedProfileChanged += (sender, args) => SetTokenViewModels();
        }

        private void SetTokenViewModels(ITranslationFactory translationFactory)
        {
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            SetTokenViewModels();
        }

        private void SetTokenViewModels()
        {
            var tokens = _tokenHelper.GetTokenListForMetadata();
            var profile = _currentSettingsProvider.SelectedProfile;

            TitleTokenViewModel = new TokenViewModel(v => profile.TitleTemplate = v, () => profile.TitleTemplate, tokens, ReplaceTokens);
            AuthorTokenViewModel = new TokenViewModel(v => profile.AuthorTemplate = v, () => profile.AuthorTemplate, tokens, ReplaceTokens);
            SubjectTokenViewModel = new TokenViewModel(v => profile.SubjectTemplate = v, () => profile.SubjectTemplate, tokens, ReplaceTokens);
            KeywordsTokenViewModel = new TokenViewModel(v => profile.KeywordTemplate = v, () => profile.KeywordTemplate, tokens, ReplaceTokens);

            RaisePropertyChanged(nameof(TitleTokenViewModel));
            RaisePropertyChanged(nameof(AuthorTokenViewModel));
            RaisePropertyChanged(nameof(SubjectTokenViewModel));
            RaisePropertyChanged(nameof(KeywordsTokenViewModel));
        }

        private string ReplaceTokens(string s)
        {
            return _tokenReplacer.ReplaceTokens(s);
        }
    }
}
