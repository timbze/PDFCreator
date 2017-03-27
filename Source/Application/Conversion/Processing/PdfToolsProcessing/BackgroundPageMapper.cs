using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class BackgroundPageMapper
    {
        private readonly int _numBackgroundPages;
        private readonly BackgroundPage _backgroundPageSettings;
        private readonly int _numCoverPages;
        private readonly int _numAttachmentPages;

        public BackgroundPageMapper(int numBackgroundPages, BackgroundPage backgroundPageSettings, int numCoverPages, int numAttachmentPages)
        {
            _numBackgroundPages = numBackgroundPages;
            _backgroundPageSettings = backgroundPageSettings;
            _numCoverPages = numCoverPages;
            _numAttachmentPages = numAttachmentPages;
        }

        public bool GetBackgroundPageNumber(int page, int numPages, out int backgroundPage)
        {
            if ((page <= 0) || (page > numPages))
                throw new ArgumentException(nameof(page));

            backgroundPage = -1;

            if (!_backgroundPageSettings.OnCover)
            {
                page -= _numCoverPages;
                numPages -= _numCoverPages;

                if (page <= 0)
                    return false;
            }

            if (!_backgroundPageSettings.OnAttachment && (page > numPages - _numAttachmentPages))
                return false;

            if (page > _numBackgroundPages)
            {
                if (_backgroundPageSettings.Repetition == BackgroundRepetition.NoRepetition)
                    return false;

                if (_backgroundPageSettings.Repetition == BackgroundRepetition.RepeatLastPage)
                {
                    backgroundPage = _numBackgroundPages;
                    return true;
                }
            }

            backgroundPage = ((page-1) % _numBackgroundPages) + 1;

            return true;
        }
    }
}
