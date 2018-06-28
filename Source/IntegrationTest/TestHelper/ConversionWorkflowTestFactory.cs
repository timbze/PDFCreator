using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace PDFCreator.TestUtilities
{
    public class ConversionWorkflowTestFactory
    {
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly AutosaveOutputFileMover _outputFileMover;
        private readonly IJobRunner _jobRunner;
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFileNameComposer _targetFileNameComposer;

        public ConversionWorkflowTestFactory(IProfileChecker profileChecker, ITargetFileNameComposer targetFileNameComposer, IJobRunner jobRunner, IJobDataUpdater jobDataUpdater, AutosaveOutputFileMover outputFileMover)
        {
            _profileChecker = profileChecker;
            _targetFileNameComposer = targetFileNameComposer;
            _jobRunner = jobRunner;
            _jobDataUpdater = jobDataUpdater;
            _outputFileMover = outputFileMover;
        }

        public ConversionWorkflow BuildWorkflow()
        {
            return new AutoSaveWorkflow(_jobDataUpdater, _jobRunner, _profileChecker, _targetFileNameComposer, _outputFileMover, new TestNotificationService());
        }
    }

    internal class TestNotificationService : INotificationService
    {
        public void ShowInfoNotification(string documentName, string documentPath)
        {
        }

        public void ShowErrorNotification(string documentName)
        {
        }
    }
}
