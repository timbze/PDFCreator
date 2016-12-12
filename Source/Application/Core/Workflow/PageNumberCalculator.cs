using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPageNumberCalculator
    {
        int GetNumberOfPages(Job job);
    }

    public class PageNumberCalculator : IPageNumberCalculator
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPdfProcessor _processor;

        public PageNumberCalculator(IPdfProcessor processor)
        {
            _processor = processor;
        }

        public int GetNumberOfPages(Job job)
        {
            var numberOfPages = 0;
            numberOfPages += NumberOfPagesFromSourceFiles(job.JobInfo);
            numberOfPages += GetNumberOfCoverPages(job.Profile.CoverPage);
            numberOfPages += GetNumberOfAttachmentPages(job.Profile.AttachmentPage);

            return numberOfPages;
        }

        private int GetNumberOfAttachmentPages(AttachmentPage attachmentPage)
        {
            var count = 0;
            if (!attachmentPage.Enabled)
                return count;

            try
            {
                count += _processor.GetNumberOfPages(attachmentPage.File);
            }
            catch
            {
                _logger.Warn("Problem detecting page number of attachment page file \"" + attachmentPage.File + "\"");
                count = 1;
            }
            return count;
        }

        private int NumberOfPagesFromSourceFiles(JobInfo jobInfo)
        {
            var pages = jobInfo.TotalPages;
            if (pages <= 0)
            {
                _logger.Warn("Problem detecting number of pages from source file(s). Set to 1.");
                pages = 1;
            }

            _logger.Debug("Number of pages from source files: " + pages);
            return pages;
        }

        private int GetNumberOfCoverPages(CoverPage coverPage)
        {
            var count = 0;

            if (!coverPage.Enabled)
                return count;

            try
            {
                count += _processor.GetNumberOfPages(coverPage.File);
            }
            catch
            {
                _logger.Warn("Problem detecting page number of cover page file \"" + coverPage.File + "\"");
                count = 1;
            }
            return count;
        }
    }
}