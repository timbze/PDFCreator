using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    [Category("LongRunning")]
    internal class PageOrientationTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("AutoRotateTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        [Test]
        public void TestAutoRotatePageByPage()
        {
            _th.Profile.PdfSettings.PageOrientation = PageOrientation.Automatic;

            _th.GenerateGsJob(PSfiles.PortraitLandscapeLandscapeLandscapePortrait, OutputFormat.Pdf);
            _th.RunGsJob();

            var reader = new PdfReader(_th.Job.OutputFiles[0]);

            Assert.AreEqual(0, reader.GetPageRotation(1), "Wrong Rotation on Page 1");
            Assert.Greater(reader.GetPageSize(1).Height, reader.GetPageSize(1).Width, "Page 1 is Landscape, all Pages used to be Portrait with Rotation (if necessary)");
            Assert.AreEqual(90, reader.GetPageRotation(2), "Wrong Rotation on Page 2");
            Assert.Greater(reader.GetPageSize(2).Height, reader.GetPageSize(2).Width, "Page 2 is Landscape, all Pages used to be Portrait with Rotation (if necessary)");
            Assert.AreEqual(90, reader.GetPageRotation(3), "Wrong Rotation on Page 3");
            Assert.Greater(reader.GetPageSize(3).Height, reader.GetPageSize(3).Width, "Page 3 is Landscape, all Pages used to be Portrait with Rotation (if necessary)");
            Assert.AreEqual(90, reader.GetPageRotation(4), "Wrong Rotation on Page 4");
            Assert.Greater(reader.GetPageSize(4).Height, reader.GetPageSize(4).Width, "Page 4 is Landscape, all Pages used to be Portrait with Rotation (if necessary)");
            Assert.AreEqual(0, reader.GetPageRotation(5), "Wrong Rotation on Page 5");
            Assert.Greater(reader.GetPageSize(5).Height, reader.GetPageSize(5).Width, "Page 5 is Landscape, all Pages used to be Portrait with Rotation (if necessary)");
        }

        [Test]
        public void TestLandscape()
        {
            _th.Profile.PdfSettings.PageOrientation = PageOrientation.Landscape;

            _th.GenerateGsJob(PSfiles.PortraitLandscapeLandscapeLandscapePortrait, OutputFormat.Pdf);
            _th.RunGsJob();

            var reader = new PdfReader(_th.Job.OutputFiles[0]);

            for (var page = 1; page <= reader.NumberOfPages; page++)
            {
                Assert.AreEqual(0, reader.GetPageRotation(page), "Rotation on Page" + page + " in Portrait-Mode");
                Assert.Greater(reader.GetPageSize(page).Width, reader.GetPageSize(page).Height, "Page " + page + "is Landscape in Portrait-Mode");
            }
        }

        [Test]
        public void TestPortrait()
        {
            _th.Profile.PdfSettings.PageOrientation = PageOrientation.Portrait;

            _th.GenerateGsJob(PSfiles.PortraitLandscapeLandscapeLandscapePortrait, OutputFormat.Pdf);
            _th.RunGsJob();

            var reader = new PdfReader(_th.Job.OutputFiles[0]);

            for (var page = 1; page <= reader.NumberOfPages; page++)
            {
                Assert.AreEqual(0, reader.GetPageRotation(page), "Rotation on Page" + page + " in Portrait-Mode");
                Assert.Greater(reader.GetPageSize(page).Height, reader.GetPageSize(1).Width, "Page " + page + "is Landscape in Portrait-Mode");
            }
        }
    }
}