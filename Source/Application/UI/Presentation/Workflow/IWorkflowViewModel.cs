using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IWorkflowViewModel
    {
        void ExecuteWorkflowStep(Job job);

        event EventHandler StepFinished;
    }
}
