using System;
using System.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class AutosaveTargetFileNameComposer : ITargetFileNameComposer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IOutputFilenameComposer _outputFilenameComposer;
        private readonly IPathUtil _pathUtil;

        public AutosaveTargetFileNameComposer(IPathUtil pathUtil, IOutputFilenameComposer outputFilenameComposer)
        {
            _pathUtil = pathUtil;
            _outputFilenameComposer = outputFilenameComposer;
        }

        public string ComposeTargetFileName(Job job)
        {
            var tr = job.TokenReplacer;
            var validName = new ValidName();
            var outputFolder = validName.MakeValidFolderName(tr.ReplaceTokens(job.Profile.TargetDirectory));
            var filePath = Path.Combine(outputFolder, _outputFilenameComposer.ComposeOutputFilename(job));

            try
            {
                filePath = _pathUtil.EllipsisForTooLongPath(filePath);
                _logger.Debug("FilePath after ellipsis: " + filePath);
            }
            catch (ArgumentException)
            {
                throw new WorkflowException("Autosave filepath is only a directory or the directory itself is already too long to append a filename under the limits of Windows (max " + _pathUtil.MAX_PATH + " characters): " + filePath);
            }

            return filePath;
        }
    }
}
