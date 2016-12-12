using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public interface IComWorkflowFactory
    {
        IConversionWorkflow BuildWorkflow(string targetFileName, IErrorNotifier errorNotifier);
    }
}
