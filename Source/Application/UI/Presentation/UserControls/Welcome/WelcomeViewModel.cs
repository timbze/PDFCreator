using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeViewModel : OverlayViewModelBase<WelcomeInteraction, WelcomeWindowTranslation>, IWhitelisted
    {
        private readonly IProcessStarter _processStarter;
        private readonly ButtonDisplayOptions _buttonDisplayOptions;
        private readonly IUserGuideHelper _userGuideHelper;

        public WelcomeViewModel(IProcessStarter processStarter, ButtonDisplayOptions buttonDisplayOptions, IUserGuideHelper userGuideHelper, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _processStarter = processStarter;
            _buttonDisplayOptions = buttonDisplayOptions;
            _userGuideHelper = userGuideHelper;

            WhatsNewCommand = new DelegateCommand(WhatsNewCommandExecute);
            FacebookCommand = new DelegateCommand(FacebookCommandExecute);
            GooglePlusCommand = new DelegateCommand(GooglePlusCommandExecute);
        }

        public bool HideSocialMediaButtons => _buttonDisplayOptions.HideSocialMediaButtons;

        public DelegateCommand WhatsNewCommand { get; }
        public DelegateCommand FacebookCommand { get; }
        public DelegateCommand GooglePlusCommand { get; }

        public void WhatsNewCommandExecute(object obj)
        {
            _userGuideHelper.ShowHelp(HelpTopic.WhatsNew);
        }

        public void FacebookCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.Facebook);
        }

        public void GooglePlusCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.GooglePlus);
        }

        private void ShowUrlInBrowser(string url)
        {
            try
            {
                _processStarter.Start(url);
            }
            catch (Exception)
            {
            }
        }

        public override string Title => Translation.Title;
    }
}
