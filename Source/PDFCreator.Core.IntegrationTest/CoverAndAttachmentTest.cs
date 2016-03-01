using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    class CoverAndAttachmentTest
    {
        private TestHelper _th;

        private string _coverFile;
        private string _attachmentFile;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("CoverAndAttachmentTest");

            _coverFile = _th.GenerateTestFile(TestFile.Cover2PagesPDF);
            _attachmentFile = _th.GenerateTestFile(TestFile.Attachment3PagesPDF);
        }
        
        [TearDown]
        public void CleanUp()
        {
            if (_th != null)
                _th.CleanUp();
        }

        [Test]
        public void AddCoverPageToPDF()
        {
            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover=false;
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.RunGsJob();
            var reader = new PdfReader(_th.Job.OutputFiles[0]);
            Assert.AreEqual(3, reader.NumberOfPages, "Wrong number of pages. Cover(2) + Document(1) = 3");
            string pageText = PdfTextExtractor.GetTextFromPage(reader, 1);
            Assert.AreEqual(pageText, "Cover 1 ", "1. page is not 1. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 2);
            Assert.AreEqual(pageText, "Cover 2 ", "2. page is not 2. cover page");

            reader.Close();
        }

        [Test]
        public void AddAttachmentPageToPDF()
        {
            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.RunGsJob();
            var reader = new PdfReader(_th.Job.OutputFiles[0]);
            Assert.AreEqual(4, reader.NumberOfPages, "Wrong number of pages. Document(1) + Attachment(3) = 4");
            string pageText = PdfTextExtractor.GetTextFromPage(reader, 2);
            Assert.AreEqual("Attachment 1 ", pageText, "2. page is not the 1. attachment page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 3);
            Assert.AreEqual("Attachment 2 ", pageText, "3. page is not the 3. attachment page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
            Assert.AreEqual("Attachment 3 ", pageText, "4. page is not the 3. attachment page");

            reader.Close();
        }

        [Test]
        public void AddCoverAndAttachmentPageToPDF()
        {
            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;
            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.RunGsJob();
            var reader = new PdfReader(_th.Job.OutputFiles[0]);
            Assert.AreEqual(6, reader.NumberOfPages, "Wrong number of pages. Cover(2) + Document(1) + Attachment(3) = 6");
            string pageText = PdfTextExtractor.GetTextFromPage(reader, 1);
            Assert.AreEqual(pageText, "Cover 1 ", "1. page is not the 1. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 2);
            Assert.AreEqual(pageText, "Cover 2 ", "2. page is not the 2. cover page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
            Assert.AreEqual("Attachment 1 ", pageText, "4. page is not the 1. attachment page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
            Assert.AreEqual("Attachment 2 ", pageText, "5. page is not the 3. attachment page");
            pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
            Assert.AreEqual("Attachment 3 ", pageText, "6. page is not the 3. attachment page");

            reader.Close();
        }
    }
}