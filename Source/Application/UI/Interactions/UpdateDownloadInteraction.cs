using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class UpdateDownloadInteraction : IInteraction
    {
        public UpdateDownloadInteraction(string downloadUrl)
        {
            DownloadUrl = downloadUrl;
        }

        public string DownloadUrl { get; set; }

        public string DownloadedFile { get; set; }

        public bool Success { get; set; }
    }
}