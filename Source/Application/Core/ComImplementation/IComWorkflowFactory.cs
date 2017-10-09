using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public interface IComWorkflowFactory
    {
        IConversionWorkflow BuildWorkflow(string targetFileName);
    }
}
