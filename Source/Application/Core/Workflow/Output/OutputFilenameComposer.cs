using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public interface IOutputFilenameComposer
    {
        string ComposeOutputFilename(Job job);
    }

    public class OutputFilenameComposer : IOutputFilenameComposer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPathUtil _pathUtil;

        public OutputFilenameComposer(IPathUtil pathUtil)
        {
            _pathUtil = pathUtil;
        }

        public string ComposeOutputFilename(Job job)
        {
            var validName = new ValidName();
            var outputFilename = validName.MakeValidFileName(job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate));

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

            if (outputFilename.Length > _pathUtil.MAX_PATH)
            {
                outputFilename = _pathUtil.EllipsisForPath(outputFilename, 250);
            }

            return outputFilename;
        }
    }
}