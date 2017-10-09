using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.StartupInterface;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComDependencies
    {
        public ComDependencies(PdfCreatorAdapter pdfCreatorAdapter, QueueAdapter queueAdapter, IList<IStartupCondition> startupConditions)
        {
            PdfCreatorAdapter = pdfCreatorAdapter;
            QueueAdapter = queueAdapter;

            // This is a dummy to maintain an active reference to the wrapper assembly
            var t = typeof(ComWrapper.PdfCreatorObj);
        }

        public PdfCreatorAdapter PdfCreatorAdapter { get; private set; }
        public QueueAdapter QueueAdapter { get; private set; }
    }
}
