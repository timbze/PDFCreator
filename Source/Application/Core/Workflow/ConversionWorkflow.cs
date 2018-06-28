using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using System;

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
    public abstract class ConversionWorkflow : IConversionWorkflow
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The step the workflow currently is in
        /// </summary>
        protected WorkflowResult WorkflowResult;

        protected abstract IJobDataUpdater JobDataUpdater { get; }

        public ErrorCode? LastError { get; private set; }

        public event EventHandler JobFinished;

        /// <summary>
        ///     Runs all steps and user interaction that is required during the conversion
        /// </summary>
        public WorkflowResult RunWorkflow(Job job)
        {
            try
            {
                PrepareAndRun(job);
            }
            catch (AbortWorkflowException ex)
            {
                // we need to clean up the job when it was cancelled
                _logger.Warn(ex.Message + " No output will be created.");
                WorkflowResult = WorkflowResult.AbortedByUser;
            }
            catch (WorkflowException ex)
            {
                _logger.Error(ex.Message);
                WorkflowResult = WorkflowResult.Error;
            }
            catch (ProcessingException ex)
            {
                _logger.Error("Error " + ex.ErrorCode + ": " + ex.Message);
                LastError = ex.ErrorCode;

                HandleError(ex.ErrorCode);
                WorkflowResult = WorkflowResult.Error;
            }
            catch (ManagePrintJobsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                WorkflowResult = WorkflowResult.Error;
                throw;
            }

            return WorkflowResult;
        }

        protected abstract void DoWorkflowWork(Job job);

        private void PrepareAndRun(Job job)
        {
            WorkflowResult = WorkflowResult.Init;

            _logger.Debug("Starting conversion...");

            var originalMetadata = job.JobInfo.Metadata.Copy();
            job.InitMetadataWithTemplates();

            JobDataUpdater.UpdateTokensAndMetadata(job);

            try
            {
                DoWorkflowWork(job);

                WorkflowResult = WorkflowResult.Finished;
            }
            catch (ManagePrintJobsException)
            {
                // revert metadata changes and rethrow exception
                job.JobInfo.Metadata = originalMetadata;
                throw;
            }
            finally
            {
                OnJobFinished(EventArgs.Empty);
            }
        }

        protected void OnJobFinished(EventArgs e)
        {
            JobFinished?.Invoke(this, e);
        }

        protected virtual void HandleError(ErrorCode errorCode)
        {
        }
    }
}
