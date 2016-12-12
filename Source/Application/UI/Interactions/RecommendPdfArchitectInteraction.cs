using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class RecommendPdfArchitectInteraction : IInteraction
    {
        public RecommendPdfArchitectInteraction(bool showViewerWarning)
        {
            ShowViewerWarning = showViewerWarning;
        }

        public bool ShowViewerWarning { get; set; }
    }
}