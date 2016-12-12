using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IConversionProgress
    {
        void Show(Job job);
    }

    public class AutosaveConversionProgress : IConversionProgress
    {
        public void Show(Job job)
        {
            
        }
    }
}