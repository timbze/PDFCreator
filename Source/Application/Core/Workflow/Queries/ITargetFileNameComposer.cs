using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface ITargetFileNameComposer
    {
        string ComposeTargetFileName(Job job);
    }
}
