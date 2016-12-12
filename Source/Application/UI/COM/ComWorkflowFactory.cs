using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComWorkflowFactory : IComWorkflowFactory
    {
        private readonly Container _container;

        public ComWorkflowFactory(Container container)
        {
            _container = container;
        }

        public IConversionWorkflow BuildWorkflow(string targetFileName, IErrorNotifier errorNotifier)
        {
            var profileChecker = _container.GetInstance<IProfileChecker>();
            var targetFileNameComposer = new ComTargetFileNameComposer(targetFileName);
            var jobRunner = _container.GetInstance<IJobRunner>();
            var jobDataUpdater = _container.GetInstance<IJobDataUpdater>();

            return new ConversionWorkflow(profileChecker, targetFileNameComposer, jobRunner, jobDataUpdater, errorNotifier);
        }
    }
}
