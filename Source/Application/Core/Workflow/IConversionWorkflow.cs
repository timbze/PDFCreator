using System;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IConversionWorkflow
    {
        IErrorNotifier ErrorNotifier { get; }
        Job Job { get; }

        event EventHandler JobFinished;

        WorkflowResult RunWorkflow(Job job);
    }
}