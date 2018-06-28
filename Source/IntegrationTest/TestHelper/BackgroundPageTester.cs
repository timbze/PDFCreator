using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace PDFCreator.TestUtilities
{
    public class BackgroundPageTester
    {
        public static void BackgroundOnPage(Job job, int pageNumber = 1)
        {
            BackgroundOnPage(job.OutputFiles[0], pageNumber);
        }

        private static void BackgroundOnPage(string pdfFile, int pageNumber = 1)
        {
            using (var reader = new PdfReader(pdfFile))
            {
                var pageText = PdfTextExtractor.GetTextFromPage(reader, pageNumber).Replace("\n", "").Replace(" ", "").Replace("1", "");
                Assert.IsTrue(pageText.Contains("Background"), "Did not add background to " + pageNumber + ". page of document.");
            }
        }
    }
}
