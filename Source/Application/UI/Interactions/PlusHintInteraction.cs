using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class PlusHintInteraction : IInteraction
    {
        public PlusHintInteraction(int currentJobCount)
        {
            CurrentJobCount = currentJobCount;
        }

        public int CurrentJobCount { get; set; }
    }
}