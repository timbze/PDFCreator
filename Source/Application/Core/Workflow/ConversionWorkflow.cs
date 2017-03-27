using System;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.Core.Workflow
{
    /// <summary>
    ///     Defines the different stats the workflow can be in
    /// </summary>
    public enum WorkflowResult
    {
        Init,
        AbortedByUser,
        Error,
        Finished
    }

    /// <summary>
    ///     The ConversionWorkflow class handles all required steps to convert a PostScript file.
    ///     If required (i.e. during interactive conversion), the respective requests are invoked
    ///     on the IWorkflowInformationQuery implementation.
    /// </summary>
    public class ConversionWorkflow : IConversionWorkflow
    {
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly IJobRunner _jobRunner;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFileNameComposer _targetFileNameComposer;

        /// <summary>
        ///     The step the workflow currently is in
        /// </summary>
        private WorkflowResult _workflowResult;

        public ConversionWorkflow(IProfileChecker profileChecker, ITargetFileNameComposer targetFileNameComposer, IJobRunner jobRunner, IJobDataUpdater jobDataUpdater, IErrorNotifier errorNotifier)
        {
            _profileChecker = profileChecker;
            _targetFileNameComposer = targetFileNameComposer;
            _jobRunner = jobRunner;
            _jobDataUpdater = jobDataUpdater;
            ErrorNotifier = errorNotifier;
        }

        public IErrorNotifier ErrorNotifier { get; }

        /// <summary>
        ///     The job that is created during the workflow
        /// </summary>
        public Job Job { get; private set; }

        public event EventHandler JobFinished;

        /// <summary>
        ///     Runs all steps and user interaction that is required during the conversion
        /// </summary>
        public WorkflowResult RunWorkflow(Job job)
        {
            Job = job;
            try
            {
                try
                {
                    DoWorkflowWork();
                }
                catch (AbortWorkflowException ex)
                {
                    // we need to clean up the job when it was cancelled
                    _logger.Warn(ex.Message + " No output will be created.");
                    _workflowResult = WorkflowResult.AbortedByUser;
                }
            }
            catch (WorkflowException ex)
            {
                _logger.Error(ex.Message);
                _workflowResult = WorkflowResult.Error;
            }
            catch (ProcessingException ex)
            {
                 _logger.Error("Error " + ex.ErrorCode + ": " + ex.Message);
                ErrorNotifier.Notify(new ActionResult(ex.ErrorCode));
                                  _workflowResult = WorkflowResult.Error;
            }
            catch (ManagePrintJobsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _workflowResult = WorkflowResult.Error;
            }

            return _workflowResult;
        }

        private void DoWorkflowWork()
        {
            _workflowResult = WorkflowResult.Init;

            _logger.Debug("Starting conversion...");

            var originalMetadata = Job.JobInfo.Metadata.Copy();
            Job.InitMetadataWithTemplates();

            _jobDataUpdater.UpdateTokensAndMetadata(Job);

            _logger.Debug("Querying the place to save the file");

            
            try
            {
                _targetFileNameComposer.ComposeTargetFileName(Job);
            }
            catch (ManagePrintJobsException)
            {
                // revert metadata changes and rethrow exception
                Job.JobInfo.Metadata = originalMetadata;
                throw;
            }

            var preCheck = _profileChecker.ProfileCheck(Job.Profile, Job.Accounts);
            if (!preCheck)
                throw new ProcessingException("Invalid Profile", preCheck[0]);

            _logger.Debug("Output filename template is: {0}", Job.OutputFilenameTemplate);
            _logger.Debug("Output format is: {0}", Job.Profile.OutputFormat);

            _logger.Info("Converting " + Job.OutputFilenameTemplate);

            // Can throw ProcessingException
            _jobRunner.RunJob(Job);

            _workflowResult = WorkflowResult.Finished;
            OnJobFinished(EventArgs.Empty);
        }

        private void OnJobFinished(EventArgs e)
        {
            JobFinished?.Invoke(this, e);
        }
    }
}
