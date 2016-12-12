using System;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class WelcomeWindowViewModel : InteractionAwareViewModelBase<WelcomeInteraction>
    {
        private readonly IProcessStarter _processStarter;
        private readonly ButtonDisplayOptions _buttonDisplayOptions;
        private readonly IUserGuideHelper _userGuideHelper;

        public WelcomeWindowViewModel(IProcessStarter processStarter, ButtonDisplayOptions buttonDisplayOptions, IUserGuideHelper userGuideHelper)
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
    }
}