using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using System;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public sealed class AutoSaveWorkflow : ConversionWorkflow
    {
        private readonly IJobRunner _jobRunner;
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFileNameComposer _targetFileNameComposer;
        private readonly AutosaveOutputFileMover _outputFileMover;
        private readonly INotificationService _notificationService;

        public AutoSaveWorkflow(IJobDataUpdater jobDataUpdater, IJobRunner jobRunner, IProfileChecker profileChecker,
            ITargetFileNameComposer targetFileNameComposer, AutosaveOutputFileMover outputFileMover, INotificationService notificationService)
        {
            JobDataUpdater = jobDataUpdater;
            _jobRunner = jobRunner;
            _profileChecker = profileChecker;
            _targetFileNameComposer = targetFileNameComposer;
            _outputFileMover = outputFileMover;
            _notificationService = notificationService;
        }

        protected override IJobDataUpdater JobDataUpdater { get; }

        protected override void DoWorkflowWork(Job job)
        {
            var documentName = job.JobInfo.Metadata.Title;

            try
            {
                job.OutputFilenameTemplate = _targetFileNameComposer.ComposeTargetFileName(job);

                var preCheck = _profileChecker.ProfileCheck(job.Profile, job.Accounts);
                if (!preCheck)
                    throw new ProcessingException("Invalid Profile", preCheck[0]);

                job.Passwords = JobPasswordHelper.GetJobPasswords(job.Profile, job.Accounts);

                // Can throw ProcessingException
                _jobRunner.RunJob(job, _outputFileMover);

                WorkflowResult = WorkflowResult.Finished;
                OnJobFinished(EventArgs.Empty);

                documentName = Path.GetFileName(job.OutputFiles.First());

                _notificationService?.ShowInfoNotification(documentName);
            }
            catch (Exception)
            {
                _notificationService?.ShowErrorNotification(documentName);

                throw;
            }
        }
    }
}
