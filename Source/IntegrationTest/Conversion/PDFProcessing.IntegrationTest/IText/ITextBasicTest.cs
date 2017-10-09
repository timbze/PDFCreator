using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.IText
{
    internal class ITextBasicTest : PdfProcessingBasicTestBase
    {
        protected override IPdfProcessor BuildPdfProcessor()
        {
            return new ITextPdfProcessor(new FileWrap());
        }

        protected override void FinalizePdfProcessor()
        {
        }
    }
}
