using System;
using System.IO;
using NLog;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveFileNameQuery : IRetypeFileNameQuery, IFileNameQuery
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IOutputFilenameComposer _outputFilenameComposer;
        private readonly InteractiveWorkflowTranslation _translation;

        public InteractiveFileNameQuery(InteractiveWorkflowTranslation translation, IInteractionInvoker interactionInvoker, IOutputFilenameComposer outputFilenameComposer)
        {
            _translation = translation;
            _interactionInvoker = interactionInvoker;
            _outputFilenameComposer = outputFilenameComposer;
        }

        public QueryResult<string> GetFileName(Job job)
        {
            var interaction = CreateFileNameInteraction(job);
            var result = InvokeInteraction(interaction, job, true);

            return result;
        }

        public QueryResult<string> RetypeFileName(Job job)
        {
            NotifyUserAboutRetype(job.OutputFilenameTemplate);

            var interaction = CreateRetypeInteraction(job);

            var result = InvokeInteraction(interaction, job, false);
            return result;
        }

        private SaveFileInteraction CreateFileNameInteraction(Job job)
        {
            var interaction = new SaveFileInteraction();

            interaction.Title = _translation.SelectDestination;
            interaction.Filter = GetAllFilters();
            interaction.FilterIndex = (int) job.Profile.OutputFormat + 1;
            interaction.OverwritePrompt = true;
            interaction.ForceTopMost = true;

            interaction.FileName = _outputFilenameComposer.ComposeOutputFilename(job);

            if (job.Profile.SaveDialog.SetDirectory)
            {
                interaction.InitialDirectory = CreateTargetDirectory(job);
            }

            return interaction;
        }

        private string CreateTargetDirectory(Job job)
        {
            var validName = new ValidName();
            var saveDirectory = validName.MakeValidFolderName(job.TokenReplacer.ReplaceTokens(job.Profile.SaveDialog.Folder));

            var directoryHelper = new DirectoryHelper(saveDirectory);
            job.OnJobCompleted += (obj, sender) => directoryHelper.DeleteCreatedDirectories();

            if (directoryHelper.CreateDirectory())
            {
                _logger.Debug("Set directory in save file dialog: " + saveDirectory);
                return saveDirectory;
            }


            _logger.Warn( "Could not create directory for save file dialog. It will be opened with default save location."); 
            return "";
        }

        private QueryResult<string> InvokeInteraction(SaveFileInteraction interaction, Job job, bool canChangeFormat)
        {
            var result = new QueryResult<string> {Success = false};

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return result;

            if (canChangeFormat)
            {
                job.Profile.OutputFormat = (OutputFormat) interaction.FilterIndex - 1; 
            }

            try
            {
                var outputFile = interaction.FileName;
                var outputFormatHelper = new OutputFormatHelper();
                if (!outputFormatHelper.HasValidExtension(outputFile, job.Profile.OutputFormat))
                    outputFile = outputFormatHelper.EnsureValidExtension(outputFile, job.Profile.OutputFormat);

                result.Success = true;
                result.Data = outputFile;
                return result;
            }
            catch (PathTooLongException)
            {
                NotifyUserAboutPathTooLong();
                return InvokeInteraction(interaction, job, canChangeFormat);
            }
        }

        private void NotifyUserAboutPathTooLong()
        {
            _logger.Error("Filename (+ path) from savefile dialog is too long.");
            var message = _translation.SelectedPathTooLong;
            var title = _translation.SelectDestination;

            ShowMessage(message, title, MessageOptions.OK, MessageIcon.Warning);
        }

        private SaveFileInteraction CreateRetypeInteraction(Job job)
        {
            var interaction = new SaveFileInteraction();

            interaction.Title = _translation.SelectDestination;

            interaction.Filter = GetFilterForOutputFormat(job.Profile.OutputFormat);
            interaction.FilterIndex = 1;

            interaction.OverwritePrompt = true;
            var currentFileName = job.OutputFilenameTemplate;
            interaction.FileName = Path.GetFileName(currentFileName);
            interaction.InitialDirectory = Path.GetDirectoryName(currentFileName);
            interaction.ForceTopMost = true;

            return interaction;
        }

        private string GetAllFilters()
        {
            var filter = _translation.PdfFile + @" (*.pdf)|*.pdf";
            filter += @"|" + _translation.PdfA1bFile + @" (*.pdf)|*.pdf";
            filter += @"|" + _translation.PdfA2bFile + @" (*.pdf)|*.pdf";
            filter += @"|" + _translation.PdfXFile + @" (*.pdf)|*.pdf";
            filter += @"|" + _translation.JpegFile + @" (*.jpg)|*.jpg;*.jpeg;";
            filter += @"|" + _translation.PngFile + @" (*.png)|*.png;";
            filter += @"|" + _translation.TiffFile + @" (*.tif)|*.tif;*.tiff";
            filter += @"|" + _translation.TextFile + @" (*.txt)|*.txt;";
            return filter;
        }

        private string GetFilterForOutputFormat(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.Pdf:
                    return _translation.PdfFile + @" (*.pdf)|*.pdf";
                case OutputFormat.PdfA1B:
                    return _translation.PdfA1bFile + @" (*.pdf)|*.pdf";
                case OutputFormat.PdfA2B:
                    return _translation.PdfA2bFile + @" (*.pdf)|*.pdf";
                case OutputFormat.PdfX:
                    return _translation.PdfXFile + @" (*.pdf)|*.pdf";
                case OutputFormat.Jpeg:
                    return _translation.JpegFile + @" (*.jpg)|*.jpg;*.jpeg;";
                case OutputFormat.Png:
                    return _translation.PngFile+ @" (*.png)|*.png;";
                case OutputFormat.Tif:
                    return _translation.TiffFile + @" (*.tif)|*.tif;*.tiff";
                case OutputFormat.Txt:
                    return _translation.TextFile + @" (*.txt)|*.txt;";
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputFormat), outputFormat, null);
            }
        }

        private void NotifyUserAboutRetype(string currentFileName)
        {
            const string title = "PDFCreator";

            var messageText = _translation.RetypeFilenameMessage;
            var message = $"{currentFileName} \r\n{messageText}";

            ShowMessage(message, title, MessageOptions.OK, MessageIcon.Warning);
        }

        private void ShowMessage(string text, string title, MessageOptions buttons, MessageIcon icon)
        {
            var messageInteraction = new MessageInteraction(text, title, buttons, icon);
            _interactionInvoker.Invoke(messageInteraction);
        }
    }
}