using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    [Category("LongRunning")]
    internal class JpegTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PNGTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        public void MakeJpegTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.RunGsJob();

            foreach (var file in _th.Job.OutputFiles)
            {
                var img = Image.FromFile(file);
                Assert.AreEqual(img.HorizontalResolution, img.VerticalResolution, "vertical resolution != horizontal resolution");
                Assert.AreEqual(_th.Profile.JpegSettings.Dpi, img.VerticalResolution, 0.013, "vertical resolution != choosen dpi");

                switch (_th.Profile.JpegSettings.Color)
                {
                    case JpegColor.Color24Bit:
                        Assert.AreEqual(PixelFormat.Format24bppRgb, img.PixelFormat, "Wrong PixelFormat/ColorScheme");
                        break;
                    case JpegColor.Gray8Bit:
                        Assert.AreEqual(PixelFormat.Format8bppIndexed, img.PixelFormat, "Wrong PixelFormat/ColorScheme");
                        break;
                }
                img.Dispose();
            }
        }

        //one test for each colour-scheme, each with different resolution
        [Test]
        public void TestColor()
        {
            _th.Profile.JpegSettings.Color = JpegColor.Color24Bit;
            _th.Profile.JpegSettings.Dpi = 223;
            _th.Profile.JpegSettings.Quality = 100;
            MakeJpegTest();
        }

        [Test]
        public void TestGray()
        {
            _th.Profile.JpegSettings.Color = JpegColor.Gray8Bit;
            _th.Profile.JpegSettings.Dpi = 220;
            _th.Profile.JpegSettings.Quality = 50;
            MakeJpegTest();
        }
    }
}