using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    internal class BackgroundPageTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("BackgroundPageTest");

            _backgroundFile = _th.GenerateTestFile(TestFile.Background3PagesPDF);
            _coverFile = _th.GenerateTestFile(TestFile.Cover2PagesPDF);
            _attachmentFile = _th.GenerateTestFile(TestFile.Attachment3PagesPDF);
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private string _backgroundFile;
        private string _coverFile;
        private string _attachmentFile;

        [Test]
        public void TestNoRepetition()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
            }
        }

        [Test]
        public void TestNoRepetitionAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestNoRepetitionAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "2. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "3. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "2. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "3. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background page on 1. cover page");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background page on 2. cover page");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4);
                Assert.AreEqual(" ", pageText, "2. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5);
                Assert.AreEqual(" ", pageText, "3. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestNoRepetitionCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.NoRepetition;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "1. page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6);
                Assert.AreEqual(" ", pageText, "4. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7);
                Assert.AreEqual(" ", pageText, "5. document page is not empty");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8);
                Assert.AreEqual(" ", pageText, "6. document page is not empty");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPages()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatAllPagesAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPagesAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatAllPagesCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatAllPages;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPage()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatLastPageAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPageAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //document
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "7. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "8. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "9. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = true;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsTrue(pageText.Contains("Background1"), "Did not add 1. background on 1. cover page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsTrue(pageText.Contains("Background2"), "Did not add 2. background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithoutBackgroundAttachmentWithBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = true;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsTrue(pageText.Contains("Background3"), "Did not add 3. background on 3. attachment page.");
            }
        }

        [Test]
        public void TestRepeatLastPageCoverWithoutBackgroundAttachmentWithoutBackground()
        {
            _th.Profile.BackgroundPage.Enabled = true;
            _th.Profile.BackgroundPage.File = _backgroundFile;
            _th.Profile.BackgroundPage.Repetition = BackgroundRepetition.RepeatLastPage;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _coverFile;
            _th.Profile.BackgroundPage.OnCover = false;

            _th.Profile.AttachmentPage.Enabled = true;
            _th.Profile.AttachmentPage.File = _attachmentFile;
            _th.Profile.BackgroundPage.OnAttachment = false;

            _th.GenerateGsJob(PSfiles.SixEmptyPages, OutputFormat.Pdf);
            _th.RunGsJob();

            using (var reader = new PdfReader(_th.Job.OutputFiles[0]))
            {
                //Cover
                var pageText = PdfTextExtractor.GetTextFromPage(reader, 1).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover1"), "1. Page is not 1. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on cover.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 2).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Cover2"), "2. Page is not 2. cover page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. cover page.");
                //Document
                pageText = PdfTextExtractor.GetTextFromPage(reader, 3).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background1", pageText, "Did not add 1. background page to 1. of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 4).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background2", pageText, "Did not add 2. background page to 2. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 5).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 3. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 6).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 4. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 7).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 5. page of document.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 8).Replace("\n", "").Replace(" ", "");
                Assert.AreEqual("Background3", pageText, "Did not add 3. background page to 6. page of document.");
                //attachment
                pageText = PdfTextExtractor.GetTextFromPage(reader, 9).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment1"), "9. page is not 1. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 1. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 10).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment2"), "10. page is not 2. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 2. attachment page.");
                pageText = PdfTextExtractor.GetTextFromPage(reader, 11).Replace("\n", "").Replace(" ", "");
                Assert.IsTrue(pageText.Contains("Attachment3"), "11. page is not 3. attachment page.");
                Assert.IsFalse(pageText.Contains("Background"), "Unwanted background on 3. attachment page.");
            }
        }
    }
}
