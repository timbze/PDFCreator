using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class ConversionProgressInteraction : IInteraction
    {
        public ConversionProgressInteraction(Job job)
        {
            Job = job;
        }

        public Job Job { get; set; }
    }
}