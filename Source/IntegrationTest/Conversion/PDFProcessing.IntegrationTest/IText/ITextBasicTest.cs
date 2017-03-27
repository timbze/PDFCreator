using SystemWrapper.IO;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.IText
{
    class ITextBasicTest : PdfProcessingBasicTestBase
    {
        protected override IPdfProcessor BuildPdfProcessor(IProcessingPasswordsProvider passwordsProvider)
        {
            return new ITextPdfProcessor(new FileWrap(), passwordsProvider);
        }
    }
}
