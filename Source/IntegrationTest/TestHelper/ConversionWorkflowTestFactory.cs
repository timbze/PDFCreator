using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace PDFCreator.TestUtilities
{
    public class ConversionWorkflowTestFactory
    {
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly IErrorNotifier _errorNotifier;
        private readonly IJobRunner _jobRunner;
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFileNameComposer _targetFileNameComposer;

        public ConversionWorkflowTestFactory(IProfileChecker profileChecker, ITargetFileNameComposer targetFileNameComposer, IJobRunner jobRunner, IJobDataUpdater jobDataUpdater, IErrorNotifier errorNotifier)
        {
            _profileChecker = profileChecker;
            _targetFileNameComposer = targetFileNameComposer;
            _jobRunner = jobRunner;
            _jobDataUpdater = jobDataUpdater;
            _errorNotifier = errorNotifier;
        }

        public ConversionWorkflow BuildWorkflow()
        {
            return new ConversionWorkflow(_profileChecker, _targetFileNameComposer, _jobRunner, _jobDataUpdater, _errorNotifier);
        }
    }
}