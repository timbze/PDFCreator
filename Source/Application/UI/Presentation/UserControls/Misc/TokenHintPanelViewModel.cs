using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class TokenHintPanelViewModel : TranslatableViewModelBase<TokenHintPanelTranslation>, IWhitelisted
    {
        private string _textWrapper = "";

        public TokenHintPanelViewModel(ITranslationUpdater translationUpdater, TokenHelper tokenHelper, ShowUserGuideCommand userGuideCommand) : base(translationUpdater)
        {
            TokenHelper = tokenHelper;
            UserGuideCommand = userGuideCommand;
            userGuideCommand.Init(HelpTopic.Tokens);
        }

        public string TextWrapper
        {
            get { return _textWrapper; }
            set
            {
                _textWrapper = value;
                RaisePropertyChanged(nameof(TextWrapper));
            }
        }

        public TokenHelper TokenHelper { get; private set; }
        public ShowUserGuideCommand UserGuideCommand { get; }

        public void OnTextChanged(string text)
        {
            TextWrapper = text;
        }
    }

    public class DesignTimeTokenHintPanelViewModel : TokenHintPanelViewModel
    {
        public DesignTimeTokenHintPanelViewModel() : base(new DesignTimeTranslationUpdater(), null, new ShowUserGuideCommand(null))
        {
        }
    }
}
