using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateManagerTranslation : ITranslatable
    {
        private string ApplicationUpdate { get; set; } = "{0} Update";
        public string AskLater { get; private set; } = "Ask Later";
        public string DownloadErrorMessage { get; private set; } = "There was a problem during the download. Do you want to download the update from the website instead?";
        public string DownloadHashErrorMessage { get; private set; } = "The MD5 hash of the downloaded file does not match the expected hash. It is possible that the file has been damaged during the download. Do you want to retry the download?";
        public string ErrorMessage { get; private set; } = "Failure in update-process.\nPlease check your internet-connection and retry later.";
        private string NewUpdateMessage { get; set; } = "The new version {0} is available.\nWould you like to download the new version?";
        public string NoUpdateMessage { get; private set; } = "You already have the most recent version.";
        public string SkipVersion { get; private set; } = "Skip version";
        public string Install { get; private set; } = "Install";
        public string WhatsNew { get; private set; } = "What's new?";

        public string NewUpdateIsAvailable { get; private set; } = "A new Update is available";

        public string GetNewUpdateMessage(string interactionAvailableVersion)
        {
            return string.Format(NewUpdateMessage, interactionAvailableVersion);
        }

        public string GetFormattedTitle(string applicationName)
        {
            return string.Format(ApplicationUpdate, applicationName);
        }
    }
}
