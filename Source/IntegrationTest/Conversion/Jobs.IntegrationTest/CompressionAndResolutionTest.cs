using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    internal class CompressionAndResolutionTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("CompressionAndResolutionTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private void AssertImageSizes(ImageData image, string filter, int fileSize, int width)
        {
            Assert.AreEqual(filter, image.Filter);
            Assert.AreEqual(width, image.Width);
            // do asserts with an allowed delta (difference) of 2
            Assert.AreEqual(fileSize, image.FileSize, 4.0);
        }

        [Test]
        public void CompressionFactor25RunLengthResample24Dpi()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegManual;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 24;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = false;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.RunLengthEncoding;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 24;

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], null, 1995, 200);
            AssertImageSizes(imagesSizes[1], "/DCTDecode", 423, 53);
            AssertImageSizes(imagesSizes[2], "/DCTDecode", 736, 53);
            AssertImageSizes(imagesSizes[3], "/DCTDecode", 661, 79);
        }

        [Test]
        public void CompressionFactorJpegMaximum_NoResample()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMaximum;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = false;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 8;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = false;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 8;

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], null, 1995, 200);
            AssertImageSizes(imagesSizes[1], "/DCTDecode", 4202, 200);
            AssertImageSizes(imagesSizes[2], "/DCTDecode", 6179, 200);
            AssertImageSizes(imagesSizes[3], "/DCTDecode", 4197, 475);
        }

        [Test]
        public void CompressionFactorJpegMinimum_NoResample()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMinimum;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = false;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 8;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = false;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 8;

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], null, 1995, 200);
            AssertImageSizes(imagesSizes[1], "/DCTDecode", 17960, 200);
            AssertImageSizes(imagesSizes[2], "/DCTDecode", 31460, 200);
            AssertImageSizes(imagesSizes[3], "/DCTDecode", 16899, 475);
        }

        [Test]
        public void CompressionZipFaxResample30Dpi()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Zip;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 24;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 30; //Below 30 dpi the ccitt fax decoding gets disabled

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], "/CCITTFaxDecode", 448, 66);
            AssertImageSizes(imagesSizes[1], "/FlateDecode", 2336, 53);
            AssertImageSizes(imagesSizes[2], "/FlateDecode", 6636, 53);
            AssertImageSizes(imagesSizes[3], "/FlateDecode", 1017, 79);
        }

        [Test]
        public void CompressionZipNoResample()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Zip;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = false;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 24;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = true;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.Zip;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = false;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 24;

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], "/FlateDecode", 1995, 200);
            AssertImageSizes(imagesSizes[1], "/FlateDecode", 29961, 200);
            AssertImageSizes(imagesSizes[2], "/FlateDecode", 89669, 200);
            AssertImageSizes(imagesSizes[3], "/FlateDecode", 11481, 475);
        }

        [Test]
        public void NoCompressionNoResample()
        {
            _th.Profile.PdfSettings.CompressColorAndGray.Enabled = false;
            _th.Profile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegManual;
            _th.Profile.PdfSettings.CompressColorAndGray.JpegCompressionFactor = 25;
            _th.Profile.PdfSettings.CompressColorAndGray.Resampling = true;
            _th.Profile.PdfSettings.CompressColorAndGray.Dpi = 8;

            _th.Profile.PdfSettings.CompressMonochrome.Enabled = false;
            _th.Profile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;
            _th.Profile.PdfSettings.CompressMonochrome.Resampling = true;
            _th.Profile.PdfSettings.CompressMonochrome.Dpi = 8;

            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.RunGsJob();

            var extractor = new ImageExtractor();
            var imagesSizes = extractor.ExtractImagesSizes(_th.Job.OutputFiles[0]);

            AssertImageSizes(imagesSizes[0], null, 1995, 200);
            AssertImageSizes(imagesSizes[1], null, 29961, 200);
            AssertImageSizes(imagesSizes[2], null, 89669, 200);
            AssertImageSizes(imagesSizes[3], null, 11481, 475);
        }
    }
}
