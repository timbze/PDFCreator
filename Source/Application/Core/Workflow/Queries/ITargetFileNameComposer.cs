using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface ITargetFileNameComposer
    {
        void ComposeTargetFileName(Job job);
    }
}