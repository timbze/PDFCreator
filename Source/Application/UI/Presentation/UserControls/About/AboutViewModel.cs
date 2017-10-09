using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewModel : TranslatableViewModelBase<AboutViewTranslation>
    {
        public ApplicationNameProvider ApplicationNameProvider { get; }

        public AboutViewModel(IVersionHelper versionHelper
            , ButtonDisplayOptions buttonDisplayOptions, ITranslationUpdater translationUpdater
            , ICommandLocator commandLocator, ApplicationNameProvider applicationNameProvider) : base(translationUpdater)
        {
            ApplicationNameProvider = applicationNameProvider;
            HideSocialMediaButtons = buttonDisplayOptions.HideSocialMediaButtons;
            VersionText = versionHelper.FormatWithBuildNumber();

            ShowManualCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General);
            ShowLicenseCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.License);

            PdfforgeWebsiteCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfforgeWebsiteUrl);
            FacebookCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Facebook);
            GooglePlusCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.GooglePlus);
        }

        public bool HideSocialMediaButtons { get; }
        public string VersionText { get; }

        public ICommand ShowManualCommand { get; }
        public ICommand ShowLicenseCommand { get; }
        public ICommand PdfforgeWebsiteCommand { get; }
        public ICommand FacebookCommand { get; }
        public ICommand GooglePlusCommand { get; }
    }
}
