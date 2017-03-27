using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    [Category("LongRunning")]
    internal class ViewerSettingsTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("ViewerSettingsTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        public PdfReader RunJobAndLoadFileToPdfReader()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.RunGsJob();
            return new PdfReader(_th.Job.OutputFiles[0]);
        }

        /// <summary>
        ///     Functions runs the job from testhelper (_th), creates a PDFReader of the resulting file
        ///     and checks the bits in the SimpleViewerPreferences.
        ///     Assert fails if the required bits are not set or if an unrequired bit was set.
        ///     The accordant bit values are:
        ///     Console.WriteLine(PdfWriter.PageModeFullScreen); //512
        ///     Console.WriteLine(PdfWriter.PageModeUseAttachments); //2048
        ///     Console.WriteLine(PdfWriter.PageModeUseNone); //64
        ///     Console.WriteLine(PdfWriter.PageModeUseOC); //1024
        ///     Console.WriteLine(PdfWriter.PageModeUseOutlines); //128
        ///     Console.WriteLine(PdfWriter.PageModeUseThumbs); //256
        ///     Console.WriteLine(PdfWriter.PageLayoutOneColumn); //2
        ///     Console.WriteLine(PdfWriter.PageLayoutSinglePage); //1
        ///     Console.WriteLine(PdfWriter.PageLayoutTwoColumnLeft); //4
        ///     Console.WriteLine(PdfWriter.PageLayoutTwoColumnRight); //8
        ///     Console.WriteLine(PdfWriter.PageLayoutTwoPageLeft); //16
        ///     Console.WriteLine(PdfWriter.PageLayoutTwoPageRight); //32
        /// </summary>
        public void DoPageViewAndDocumentViewSettingsTest(PdfReader pdfReader)
        {
            #region PageView Bits

            if (_th.Profile.PdfSettings.PageView == PageView.OneColumn)
                Assert.AreEqual(PdfWriter.PageLayoutOneColumn,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutOneColumn,
                    "PageView: Required OneColumn not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutOneColumn,
                    "PageView: Unrequired OneColumn set");

            if (_th.Profile.PdfSettings.PageView == PageView.OnePage)
                Assert.AreEqual(PdfWriter.PageLayoutSinglePage,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutSinglePage,
                    "PageView: Required OnePage not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutSinglePage,
                    "PageView: Unrequired OnePage set");

            if (_th.Profile.PdfSettings.PageView == PageView.TwoColumnsOddLeft)
                Assert.AreEqual(PdfWriter.PageLayoutTwoColumnLeft,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoColumnLeft,
                    "PageView: Required TwoPagesOddLeft not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoColumnLeft,
                    "PageView: Unrequired TwoPagesOddLeft set");

            if (_th.Profile.PdfSettings.PageView == PageView.TwoColumnsOddRight)
                Assert.AreEqual(PdfWriter.PageLayoutTwoColumnRight,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoColumnRight,
                    "PageView: Required TwoColumnsOddRight not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoColumnRight,
                    "PageView: Unrequired TwoColumnsOddRight set");

            if (_th.Profile.PdfSettings.PageView == PageView.TwoPagesOddLeft)
                Assert.AreEqual(PdfWriter.PageLayoutTwoPageLeft,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoPageLeft,
                    "PageView: Required TwoPagesOddLeft not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoPageLeft,
                    "PageView: Unrequired TwoPagesOddLeft set");

            if (_th.Profile.PdfSettings.PageView == PageView.TwoPagesOddRight)
                Assert.AreEqual(PdfWriter.PageLayoutTwoPageRight,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoPageRight,
                    "PageView: Required TwoPagesOddRight not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageLayoutTwoPageRight,
                    "PageView: Unrequired TwoPagesOddRight set");

            #endregion

            #region DocumentView Bits

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.AttachmentsPanel)
                Assert.AreEqual(PdfWriter.PageModeUseAttachments,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseAttachments,
                    "DocumentView: Required AttachmentsPanel not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseAttachments,
                    "DocumentView: Unrequired AttachmentsPanel set");

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.ContentGroupPanel)
                Assert.AreEqual(PdfWriter.PageModeUseOC, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseOC,
                    "DocumentView: Required ContentGroupPanel not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseOC,
                    "DocumentView: Unrequired ContentGroupPanel set");

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.FullScreen)
                Assert.AreEqual(PdfWriter.PageModeFullScreen,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageModeFullScreen,
                    "DocumentView: Required FullScreen Mode not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeFullScreen,
                    "DocumentView: Unrequired FullScreen Mode set");

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.NoOutLineNoThumbnailImages)
                Assert.AreEqual(PdfWriter.PageModeUseNone, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseNone,
                    "DocumentView: Required NoOutLineNoThumbnailImages not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseNone,
                    "DocumentView: Unrequired NoOutLineNoThumbnailImages set");

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.Outline)
                Assert.AreEqual(PdfWriter.PageModeUseOutlines,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseOutlines,
                    "DocumentView: Required Outline Mode not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseOutlines,
                    "DocumentView: Unrequired Outline Mode set");

            if (_th.Profile.PdfSettings.DocumentView == DocumentView.ThumbnailImages)
                Assert.AreEqual(PdfWriter.PageModeUseThumbs,
                    pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseThumbs,
                    "DocumentView: Required ThumbnailImagese not set");
            else
                Assert.AreEqual(0, pdfReader.SimpleViewerPreferences & PdfWriter.PageModeUseThumbs,
                    "DocumentView: Unrequired ThumbnailImagese set");

            #endregion
        }

        [TestCase(PageView.OneColumn)]
        [TestCase(PageView.OnePage)]
        [TestCase(PageView.TwoColumnsOddLeft)]
        [TestCase(PageView.TwoColumnsOddRight)]
        [TestCase(PageView.TwoPagesOddLeft)]
        [TestCase(PageView.TwoPagesOddRight)]
        public void PageViewTest(PageView pageView)
        {
            _th.Profile.PdfSettings.PageView = pageView;

            var pdfReader = RunJobAndLoadFileToPdfReader();
            DoPageViewAndDocumentViewSettingsTest(pdfReader);
        }

        [TestCase(DocumentView.ContentGroupPanel)]
        [TestCase(DocumentView.FullScreen)]
        [TestCase(DocumentView.NoOutLineNoThumbnailImages)]
        [TestCase(DocumentView.Outline)]
        [TestCase(DocumentView.ThumbnailImages)]
        [TestCase(DocumentView.AttachmentsPanel)]
        public void DocumentViewTest(DocumentView documentView)
        {
            _th.Profile.PdfSettings.DocumentView = documentView;

            var pdfReader = RunJobAndLoadFileToPdfReader();
            DoPageViewAndDocumentViewSettingsTest(pdfReader);
        }

        [TestCase(0, Result = 1)]
        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 3)]
        [TestCase(4, Result = 3)]
        public int ViewerStartsOnPageTest(int startPage)
        {
            _th.Profile.PdfSettings.ViewerStartsOnPage = startPage;

            var pdfreader = RunJobAndLoadFileToPdfReader();

            return GetOpenPage(pdfreader);
        }

        [TestCase(0, Result = 1)]
        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 3)]
        [TestCase(4, Result = 4)]
        [TestCase(5, Result = 4)]
        public int ViewerStartsOnPageTest_withCoverPage(int startPage)
        {
            _th.Profile.CoverPage.Enabled = true;
            var coverPage = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);
            _th.Profile.CoverPage.File = coverPage;

            _th.Profile.PdfSettings.ViewerStartsOnPage = startPage;

            var pdfreader = RunJobAndLoadFileToPdfReader();

            return GetOpenPage(pdfreader);
        }

        [TestCase(0, Result = 1)]
        [TestCase(1, Result = 1)]
        [TestCase(4, Result = 4)]
        [TestCase(5, Result = 4)]
        public int ViewerStartsOnPageTest_withAttachmentPage(int startPage)
        {
            _th.Profile.AttachmentPage.Enabled = true;
            var attachmentPage = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);
            _th.Profile.AttachmentPage.File = attachmentPage;

            _th.Profile.PdfSettings.ViewerStartsOnPage = startPage;

            var pdfreader = RunJobAndLoadFileToPdfReader();

            return GetOpenPage(pdfreader);
        }

        [TestCase(0, Result = 1)]
        [TestCase(1, Result = 1)]
        [TestCase(5, Result = 5)]
        [TestCase(6, Result = 5)]
        public int ViewerStartsOnPageTest_withCoverAndAttachmentPage(int startPage)
        {
            _th.Profile.CoverPage.Enabled = true;
            var coverPage = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);
            _th.Profile.CoverPage.File = coverPage;

            _th.Profile.AttachmentPage.Enabled = true;
            var attachmentPage = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);
            _th.Profile.AttachmentPage.File = attachmentPage;

            _th.Profile.PdfSettings.ViewerStartsOnPage = startPage;

            var pdfreader = RunJobAndLoadFileToPdfReader();

            return GetOpenPage(pdfreader);
        }

        private static int GetOpenPage(PdfReader pdfreader)

        {
            var obj = (PdfArray) pdfreader.Catalog.Get(PdfName.OPENACTION);
            var objNum = (PRIndirectReference) obj[0];
            var desiredPage = pdfreader.GetPdfObject(objNum.Number);
            int page = -1;

            for (int i = 1; i <= pdfreader.NumberOfPages; i++)
            {
                var p = pdfreader.GetPageN(i);
                if (p == desiredPage)
                    return i;
            }

            var pdfo = pdfreader.Catalog.Get(PdfName.PAGE);
            return int.Parse(pdfo.ToString());
        }
    }
}