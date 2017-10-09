using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class WorkflowStep : IWorkflowStep
    {
        private readonly AutoResetEvent _stepFinished = new AutoResetEvent(false);
        private readonly Predicate<Job> _isRequiredPredicate;
        public string NavigationUri { get; }

        public static IWorkflowStep Create<T>() where T : UserControl
        {
            return new WorkflowStep(typeof(T), job => true);
        }

        public static IWorkflowStep Create<T>(Predicate<Job> isRequiredPredicate) where T : UserControl
        {
            return new WorkflowStep(typeof(T), isRequiredPredicate);
        }

        public WorkflowStep(Type type, Predicate<Job> isRequiredPredicate)
        {
            _isRequiredPredicate = isRequiredPredicate;
            NavigationUri = type.Name;
        }

        public bool IsStepRequired(Job job)
        {
            return _isRequiredPredicate(job);
        }

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
