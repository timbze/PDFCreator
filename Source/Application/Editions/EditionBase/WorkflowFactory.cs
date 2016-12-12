using pdfforge.PDFCreator.Core.Workflow;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    internal class WorkflowFactory : IWorkflowFactory
    {
        private static readonly object LockObject = new object();
        private readonly Container _container;

        public WorkflowFactory(Container container)
        {
            _container = container;
        }

        public WorkflowModeEnum WorkflowMode { get; private set; } = WorkflowModeEnum.Autosave;

        public IConversionWorkflow CreateWorkflow(WorkflowModeEnum mode)
        {
            lock (LockObject)
            {
                WorkflowMode = mode;
                return _container.GetInstance<ConversionWorkflow>();
            }
        }
    }
}
