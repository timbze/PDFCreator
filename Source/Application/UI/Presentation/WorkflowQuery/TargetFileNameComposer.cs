using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class TargetFileNameComposer : ITargetFileNameComposer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        private readonly IPathUtil _pathUtil;

        public TargetFileNameComposer(IPathUtil pathUtil)
        {
            _pathUtil = pathUtil;
        }

        public string ComposeTargetFileName(Job job)
        {
            var tr = job.TokenReplacer;
            var outputFolder = ValidName.MakeValidFolderName(tr.ReplaceTokens(job.Profile.TargetDirectory));
            var fileName = ComposeOutputFilename(job);
            var filePath = _pathSafe.Combine(outputFolder, fileName);

            if (!job.Profile.AutoSave.Enabled)
                return filePath;

            try
            {
                filePath = _pathUtil.EllipsisForTooLongPath(filePath);
                _logger.Debug("FilePath after ellipsis: " + filePath);
            }
            catch (ArgumentException)
            {
                throw new WorkflowException("Filepath is only a directory or the directory itself is already too long to append a usefull filename under the limits of Windows (max " + _pathUtil.MAX_PATH + " characters): " + filePath);
            }

            return filePath;
        }

        public string ComposeOutputFilename(Job job)
        {
            var outputFilename = ValidName.MakeValidFileName(job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate));

            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.Pdf:
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfX:
                    outputFilename += ".pdf";
                    break;

                case OutputFormat.Jpeg:
                    outputFilename += ".jpg";
                    break;

                case OutputFormat.Png:
                    outputFilename += ".png";
                    break;

                case OutputFormat.Tif:
                    outputFilename += ".tif";
                    break;

                case OutputFormat.Txt:
                    outputFilename += ".txt";
                    break;

                default:
                    _logger.Warn("Can't find a supported Output format! File format is defaulted to .pdf");
                    outputFilename += ".pdf";
                    break;
            }

            return outputFilename;
        }
    }
}
