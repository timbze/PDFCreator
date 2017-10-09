using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public abstract class WorkflowStepBase : IWorkflowStep
    {
        private readonly AutoResetEvent _stepFinished = new AutoResetEvent(false);

        public abstract string NavigationUri { get; }

        public abstract bool IsStepRequired(Job job);

        public async Task ExecuteStep(Job job, IWorkflowViewModel workflowViewModel)
        {
            try
            {
                _stepFinished.Reset();
                workflowViewModel.StepFinished += HandleStepFinished;
                workflowViewModel.ExecuteWorkflowStep(job);

                await Task.Run(() =>
                {
                    _stepFinished.WaitOne();
                });
            }
            finally
            {
                workflowViewModel.StepFinished -= HandleStepFinished;
            }
        }

        private void HandleStepFinished(object sender, EventArgs eventArgs)
        {
            _stepFinished.Set();
        }
    }
}
