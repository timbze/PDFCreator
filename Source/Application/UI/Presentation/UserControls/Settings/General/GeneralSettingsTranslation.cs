using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class GeneralSettingsTranslation : ITranslatable
    {
        public string Ask { get; private set; } = "Ask";
        public string AddMenuIntegrationText { get; private set; } = "Add";
        public string AddToContextMenuText { get; private set; } = "Added to context menu";
        public string ChangeDefaultPrinterLabelText { get; private set; } = "Allow PDFCreator to temporarily change the default printer";
        public string CheckUpdateButtonContent { get; private set; } = "Check now";
        public string DefaultPrinterControlHeader { get; private set; } = "Default Printer";
        public string DownloadLatestVersionText { get; private set; } = "Download the latest version at:";
        public string LanguageControlHeader { get; private set; } = "Language";
        public string LanguagePreviewButtonContent { get; private set; } = "Preview";
        public string MenuIntegrationControlHeader { get; private set; } = "Explorer Context Menu Integration";
        public string NeverUpdateWarningText { get; private set; } = "Please note that you won't receive any notifications about available updates!";
        public string RemoveFromContextMenuText { get; private set; } = "Removed from context menu (Maybe you need to restart your computer)";
        public string RemoveMenuIntegrationText { get; private set; } = "Remove";
        public string SelectLanguageLabelText { get; private set; } = "Select the application language:";
        public string UpdateCheckControlHeader { get; private set; } = "Update";
        public string UpdateCheckIsRunning { get; private set; } = "Update check is already running.";
        public string UpdateCheckTitle { get; private set; } = "Update check";
        public string NoUpdateMessage { get; private set; } = "You already have the most recent version.";
        public string UpdateIntervalLabelText { get; private set; } = "Check for updates:";
        public string Yes { get; private set; } = "Yes";
        public string GeneralTabTitle { get; private set; } = "General";

        public string AskLater { get; private set; } = "Ask Later";
        public string SkipVersion { get; private set; } = "Skip version";
        public string Install { get; private set; } = "Install new Update";

        public EnumTranslation<UpdateInterval>[] UpdateIntervals { get; private set; } = EnumTranslation<UpdateInterval>.CreateDefaultEnumTranslation();

        private string NewUpdateMessage { get; set; } = "The new version {0} is available.\nWould you like to download the new version?";

        public string GetNewUpdateMessage(string interactionAvailableVersion)
        {
            return string.Format(NewUpdateMessage, interactionAvailableVersion);
        }
    }
}
