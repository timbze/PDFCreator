using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobRunner
    {
        /// <summary>
        ///     Runs the job and all actions
        /// </summary>
        void RunJob(Job abstractJob);
    }

    public class JobRunner : IJobRunner
    {
        private readonly IActionManager _actionManager;
        private readonly IConversionProgress _conversionProgress;
        private readonly IConverterFactory _converterFactory;
        private readonly IDirectory _directory;
        private readonly IJobCleanUp _jobClean;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IOutputFileMover _outputFileMover;
        private readonly IPdfProcessor _processor;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobRunner(IOutputFileMover outputFileMover, ITokenReplacerFactory tokenReplacerFactory,
            IPdfProcessor processor, IConverterFactory converterFactory, IActionManager actionManager,
            IJobCleanUp jobClean, ITempFolderProvider tempFolderProvider, IDirectory directory,
            IConversionProgress conversionProgress)
        {
            _outputFileMover = outputFileMover;
            _tokenReplacerFactory = tokenReplacerFactory;
            _processor = processor;
            _converterFactory = converterFactory;
            _actionManager = actionManager;
            _jobClean = jobClean;
            _tempFolderProvider = tempFolderProvider;
            _directory = directory;
            _conversionProgress = conversionProgress;
        }

        /// <summary>
        ///     Runs the job and all actions
        /// </summary>
        public void RunJob(Job job)
        {
            _logger.Trace("Starting job");

            SetTempFolders(job);

            try
            {
                _processor.Init(job);
                var actions = SetUpActions(job);

                _conversionProgress.Show(job);

                Convert(job);
                Process(job);
                _outputFileMover.MoveOutputFiles(job);

                if (job.OutputFiles.Count == 0)
                {
                    _logger.Error("No output files were created for unknown reason");
                    throw new ProcessingException("No output files were created for unknown reason", ErrorCode.Conversion_UnknownError);
                }

                LogOutputFiles(job);

                job.TokenReplacer = _tokenReplacerFactory.BuildTokenReplacerWithOutputfiles(job);

                CallActions(job, actions);
                CleanUp(job);

                _logger.Trace("Job finished successfully");
            }
            catch (ProcessingException ex)
            {
                _logger.Error($"The job failed: {ex.Message} ({ex.ErrorCode})");
                throw;
            }
            finally
            {
                _logger.Trace("Calling job completed event");
                job.CallJobCompleted();
            };
        }

        private void LogOutputFiles(Job job)
        {
            _logger.Trace("Created {0} output files.", job.OutputFiles.Count);
            var i = 1;
            foreach (var file in job.OutputFiles)
            {
                _logger.Trace(i + ". Output file: {0}", file);
                i++;
            }
        }

        /// <summary>
        ///     Clean up all temporary files that have been generated during the Job
        /// </summary>
        private void CleanUp(Job job)
        {
            _logger.Debug("Cleaning up after the job");
            _jobClean.DoCleanUp(job.JobTempFolder, job.JobInfo.SourceFiles, job.JobInfo.InfFile);
        }

        private void SetTempFolders(Job job)
        {
            job.JobTempFolder = Path.Combine(_tempFolderProvider.TempFolder,
                "Job_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            job.JobTempOutputFolder = Path.Combine(job.JobTempFolder, "tempoutput");
            _directory.CreateDirectory(job.JobTempFolder);
            _directory.CreateDirectory(job.JobTempOutputFolder);

            // Shorten the temp folder for GS compatibility
            job.JobTempFolder = PathHelper.GetShortPathName(job.JobTempFolder);
                //TODO remove this after upgrade to GS 9.19
        }

        private void CallActions(Job job, IEnumerable<IAction> actions)
        {
            _logger.Trace("Starting Actions");
            foreach (var action in actions)
            {
                _logger.Trace("Calling Action {0}", action.GetType().Name);

                var result = action.ProcessJob(job);

                if (!result.IsSuccess)
                {
                    throw new ProcessingException("An action failed.", result[0]);
                }

                _logger.Trace("Action {0} completed", action.GetType().Name);
            }
        }

        /// <summary>
        ///     Apply all Actions according to the configuration
        /// </summary>
        /// <param name="job"></param>
        private IEnumerable<IAction> SetUpActions(Job job)
        {
            _logger.Trace("Setting up actions");
            var actions = _actionManager.GetAllApplicableActions(job);
            return actions;
        }

        private void Convert(Job job)
        {
            var converter = _converterFactory.GetCorrectConverter(job.JobInfo.JobType);
            converter.OnReportProgress += (sender, args) => job.ReportProgress(args.Progress);

            converter.DoConversion(job);
        }

        private void Process(Job job)
        {
            if (!_processor.ProcessingRequired(job.Profile))
                return;

            _processor.ProcessPdf(job);
        }
    }
}
