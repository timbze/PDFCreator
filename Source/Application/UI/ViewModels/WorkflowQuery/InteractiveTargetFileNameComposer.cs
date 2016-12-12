using System.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveTargetFileNameComposer : ITargetFileNameComposer
    {
        private readonly IFileNameQuery _fileNameQuery;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly IOutputFilenameComposer _outputFilenameComposer;
        private readonly IPathUtil _pathUtil;
        private readonly ITempFolderProvider _tempFolderProvider;

        public InteractiveTargetFileNameComposer(IFileNameQuery fileNameQuery, IInteractionInvoker interactionInvoker, IPathUtil pathUtil, ITempFolderProvider tempFolderProvider, IJobDataUpdater jobDataUpdater, IOutputFilenameComposer outputFilenameComposer)
        {
            _fileNameQuery = fileNameQuery;
            _interactionInvoker = interactionInvoker;
            _pathUtil = pathUtil;
            _tempFolderProvider = tempFolderProvider;
            _jobDataUpdater = jobDataUpdater;
            _outputFilenameComposer = outputFilenameComposer;
        }

        public void ComposeTargetFileName(Job job)
        {
            ShowPrintDialog(job);

            if (job.SkipSaveFileDialog)
            {
                ComposeName(job);
            }
            else
            {
                QueryName(job);
            }
        }

        private void QueryName(Job job)
        {
            var result = _fileNameQuery.GetFileName(job);
            if (result.Success)
            {
                job.OutputFilenameTemplate = result.Data;
            }
            else
            {
                throw new AbortWorkflowException("Cancelled the save dialog.");
            }
        }

        private void ComposeName(Job job)
        {
            var sendFilesFolder = Path.Combine(_tempFolderProvider.TempFolder,
                "_job" + job.JobInfo.SourceFiles[0].JobId + "_SendFiles");
            Directory.CreateDirectory(sendFilesFolder);
            var filePath = Path.Combine(sendFilesFolder, _outputFilenameComposer.ComposeOutputFilename(job));
            filePath = _pathUtil.EllipsisForTooLongPath(filePath);
            job.OutputFilenameTemplate = filePath;
        }

        private void ShowPrintDialog(Job job)
        {
            if (job.Profile.SkipPrintDialog)
                return;

            var interaction = new PrintJobInteraction(job.JobInfo, job.Profile);

            _interactionInvoker.Invoke(interaction);

            if (interaction.PrintJobAction == PrintJobAction.Cancel)
                throw new AbortWorkflowException("Cancelled the PrintJob dialog.");

            if (interaction.PrintJobAction == PrintJobAction.ManagePrintJobs)
                throw new ManagePrintJobsException();

            job.Profile = interaction.Profile.Copy();

            _jobDataUpdater.UpdateTokensAndMetadata(job);

            if (interaction.PrintJobAction != PrintJobAction.EMail)
                return;

            job.SkipSaveFileDialog = true;
            job.Profile.EmailClientSettings.Enabled = true;
            job.Profile.AutoSave.Enabled = false;
            job.Profile.OpenViewer = false;
        }
    }
}