using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class MetadataViewModel : TranslatableViewModelBase<MetadataTabTranslation>, ITabViewModel
    {
        private readonly TokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;

        public string Title => Translation.Title;
        public IconList Icon => IconList.MetadataSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;

        public TokenViewModel<ConversionProfile> TitleTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> AuthorTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SubjectTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> KeywordsTokenViewModel { get; private set; }

        public MetadataViewModel(ITranslationUpdater translationUpdater, TokenHelper tokenHelper, ITokenViewModelFactory tokenViewModelFactory) : base(translationUpdater)
        {
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForMetadata());

            TitleTokenViewModel = builder.WithSelector(p => p.TitleTemplate).Build();
            AuthorTokenViewModel = builder.WithSelector(p => p.AuthorTemplate).Build();
            SubjectTokenViewModel = builder.WithSelector(p => p.SubjectTemplate).Build();
            KeywordsTokenViewModel = builder.WithSelector(p => p.KeywordTemplate).Build();

            RaisePropertyChanged(nameof(TitleTokenViewModel));
            RaisePropertyChanged(nameof(AuthorTokenViewModel));
            RaisePropertyChanged(nameof(SubjectTokenViewModel));
            RaisePropertyChanged(nameof(KeywordsTokenViewModel));
        }
    }
}
