using pdfforge.PDFCreator.Core.ComImplementation;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComDependencies
    {
        public ComDependencies(PdfCreatorAdapter pdfCreatorAdapter, QueueAdapter queueAdapter)
        {
            PdfCreatorAdapter = pdfCreatorAdapter;
            QueueAdapter = queueAdapter;
        }

        public PdfCreatorAdapter PdfCreatorAdapter { get; private set; }
        public QueueAdapter QueueAdapter { get; private set; }
    }
}
