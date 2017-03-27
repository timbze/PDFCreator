using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class AboutWindowViewModel : InteractionAwareViewModelBase<AboutWindowInteraction>
    {
        private readonly IProcessStarter _processStarter;
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly ButtonDisplayOptions _buttonDisplayOptions;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public AboutWindowViewModel(IProcessStarter processStarter, ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IUserGuideHelper userGuideHelper, ButtonDisplayOptions buttonDisplayOptions, AboutWindowTranslation translation)
        {
            _processStarter = processStarter;
            _userGuideHelper = userGuideHelper;
            _buttonDisplayOptions = buttonDisplayOptions;

            _applicationNameProvider = applicationNameProvider;
            VersionText = versionHelper.FormatWithBuildNumber();

            ShowManualCommand = new DelegateCommand(ShowManualCommandExecute);
            ShowLicenseCommand = new DelegateCommand(ShowLicenseCommandExecute);
            DonateCommand = new DelegateCommand(DonateCommandExecute);
            PdfforgeWebsiteCommand = new DelegateCommand(PdfforgeWebsiteCommandExecute);
            FacebookCommand = new DelegateCommand(FacebookCommandExecute);
            GooglePlusCommand = new DelegateCommand(GooglePlusCommandExecute);
            Translation = translation;
        }

        public  AboutWindowTranslation Translation { get; set; }

        public bool HideDonateButton => _buttonDisplayOptions.HideDonateButton;
        public bool HideSocialMediaButtons => _buttonDisplayOptions.HideSocialMediaButtons;
        public string ApplicationName => _applicationNameProvider.ApplicationName;

        public string VersionText { get; }

        public DelegateCommand ShowManualCommand { get; }
        public DelegateCommand ShowLicenseCommand { get; }
        public DelegateCommand DonateCommand { get; }
        public DelegateCommand PdfforgeWebsiteCommand { get; }
        public DelegateCommand FacebookCommand { get; }
        public DelegateCommand GooglePlusCommand { get; }

        private void ShowManualCommandExecute(object obj)
        {
            _userGuideHelper.ShowHelp(HelpTopic.General);
        }

        private void ShowLicenseCommandExecute(object obj)
        {
            _userGuideHelper.ShowHelp(HelpTopic.License);
        }

        private void DonateCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.DonateUrl);
        }

        private void PdfforgeWebsiteCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.PdfforgeWebsiteUrl);
        }

        private void FacebookCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.Facebook);
        }

        private void GooglePlusCommandExecute(object obj)
        {
            ShowUrlInBrowser(Urls.GooglePlus);
        }

        private void ShowUrlInBrowser(string url)
        {
            try
            {
                _processStarter.Start(url);
            }
            catch
            {
                // ignored
            }
        }
    }
}