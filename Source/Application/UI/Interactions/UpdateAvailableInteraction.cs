using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class UpdateAvailableInteraction : IInteraction
    {
        public UpdateAvailableInteraction(string whatsNewUrl, string availableVersion)
        {
            WhatsNewUrl = whatsNewUrl;
            AvailableVersion = availableVersion;
        }

        public string WhatsNewUrl { get; set; }
        public string AvailableVersion { get; set; }

        public UpdateAvailableResponse Response { get; set; } = UpdateAvailableResponse.Later;
    }

    public enum UpdateAvailableResponse
    {
        Install,
        Later,
        Skip
    }
}
