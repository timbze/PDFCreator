using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs
{
    [TestFixture]
    public class OutputFormatHelperTest
    {
        [SetUp]
        public void PrepareSamples()
        {
            _outputFormatHelper = new OutputFormatHelper();
        }

        private OutputFormatHelper _outputFormatHelper;

        [TestCase("test.txt", OutputFormat.Pdf)]
        [TestCase("test.txt", OutputFormat.PdfA2B)]
        [TestCase("test.txt", OutputFormat.PdfX)]
        [TestCase("test.txt", OutputFormat.Png)]
        [TestCase("test.txt", OutputFormat.Jpeg)]
        [TestCase("test.txt", OutputFormat.Tif)]
        [TestCase("test.png", OutputFormat.Pdf)]
        [TestCase("test.tif", OutputFormat.Pdf)]
        [TestCase("test.tiff", OutputFormat.Pdf)]
        [TestCase("test.jpg", OutputFormat.Pdf)]
        [TestCase("test.jpeg", OutputFormat.Pdf)]
        [TestCase("test.png", OutputFormat.PdfA2B)]
        [TestCase("test.tif", OutputFormat.PdfA2B)]
        [TestCase("test.tiff", OutputFormat.PdfA2B)]
        [TestCase("test.jpg", OutputFormat.PdfA2B)]
        [TestCase("test.jpeg", OutputFormat.PdfA2B)]
        [TestCase("test.png", OutputFormat.PdfX)]
        [TestCase("test.tif", OutputFormat.PdfX)]
        [TestCase("test.tiff", OutputFormat.PdfX)]
        [TestCase("test.jpg", OutputFormat.PdfX)]
        [TestCase("test.jpeg", OutputFormat.PdfX)]
        [TestCase("test.pdf", OutputFormat.Png)]
        [TestCase("test.tif", OutputFormat.Png)]
        [TestCase("test.tiff", OutputFormat.Png)]
        [TestCase("test.jpg", OutputFormat.Png)]
        [TestCase("test.jpeg", OutputFormat.Png)]
        [TestCase("test.pdf", OutputFormat.Jpeg)]
        [TestCase("test.tif", OutputFormat.Jpeg)]
        [TestCase("test.tiff", OutputFormat.Jpeg)]
        [TestCase("test.png", OutputFormat.Jpeg)]
        [TestCase("test.pdf", OutputFormat.Tif)]
        [TestCase("test.png", OutputFormat.Tif)]
        [TestCase("test.jpg", OutputFormat.Tif)]
        [TestCase("test.jpeg", OutputFormat.Tif)]
        [TestCase("test.jpeg", OutputFormat.Txt)]
        public void AddValidExtension_GivenBadFiles_ReturnsGoodFiles(string filename, OutputFormat outputFormat)
        {
            var fixedFile = _outputFormatHelper.EnsureValidExtension(filename, outputFormat);
            Assert.IsTrue(_outputFormatHelper.HasValidExtension(fixedFile, outputFormat));
        }

        [TestCase("test.pdf", OutputFormat.Pdf)]
        [TestCase("test.pdf", OutputFormat.PdfA2B)]
        [TestCase("test.pdf", OutputFormat.PdfX)]
        [TestCase("test.png", OutputFormat.Png)]
        [TestCase("test.tif", OutputFormat.Tif)]
        [TestCase("test.tiff", OutputFormat.Tif)]
        [TestCase("test.TiFf", OutputFormat.Tif)]
        [TestCase("test.jpg", OutputFormat.Jpeg)]
        [TestCase("test.jpeg", OutputFormat.Jpeg)]
        [TestCase("test.jPEG", OutputFormat.Jpeg)]
        [TestCase("test.txt", OutputFormat.Txt)]
        [TestCase(@"c:\MyFolder.Tiff\test.pdf", OutputFormat.Pdf)]
        public void AddValidExtension_GivenGoodFiles_ReturnsSameFiles(string filename, OutputFormat outputFormat)
        {
            Assert.AreEqual(filename, _outputFormatHelper.EnsureValidExtension(filename, outputFormat));
        }

        [TestCase("test.txt", OutputFormat.Pdf)]
        [TestCase("test.txt", OutputFormat.PdfA2B)]
        [TestCase("test.txt", OutputFormat.PdfX)]
        [TestCase("test.txt", OutputFormat.Png)]
        [TestCase("test.txt", OutputFormat.Jpeg)]
        [TestCase("test.txt", OutputFormat.Tif)]
        [TestCase("test.png", OutputFormat.Pdf)]
        [TestCase("test.tif", OutputFormat.Pdf)]
        [TestCase("test.tiff", OutputFormat.Pdf)]
        [TestCase("test.jpg", OutputFormat.Pdf)]
        [TestCase("test.jpeg", OutputFormat.Pdf)]
        [TestCase("test.png", OutputFormat.PdfA2B)]
        [TestCase("test.tif", OutputFormat.PdfA2B)]
        [TestCase("test.tiff", OutputFormat.PdfA2B)]
        [TestCase("test.jpg", OutputFormat.PdfA2B)]
        [TestCase("test.jpeg", OutputFormat.PdfA2B)]
        [TestCase("test.png", OutputFormat.PdfX)]
        [TestCase("test.tif", OutputFormat.PdfX)]
        [TestCase("test.tiff", OutputFormat.PdfX)]
        [TestCase("test.jpg", OutputFormat.PdfX)]
        [TestCase("test.jpeg", OutputFormat.PdfX)]
        [TestCase("test.pdf", OutputFormat.Png)]
        [TestCase("test.tif", OutputFormat.Png)]
        [TestCase("test.tiff", OutputFormat.Png)]
        [TestCase("test.jpg", OutputFormat.Png)]
        [TestCase("test.jpeg", OutputFormat.Png)]
        [TestCase("test.pdf", OutputFormat.Jpeg)]
        [TestCase("test.tif", OutputFormat.Jpeg)]
        [TestCase("test.tiff", OutputFormat.Jpeg)]
        [TestCase("test.png", OutputFormat.Jpeg)]
        [TestCase("test.pdf", OutputFormat.Tif)]
        [TestCase("test.png", OutputFormat.Tif)]
        [TestCase("test.jpg", OutputFormat.Tif)]
        [TestCase("test.jpeg", OutputFormat.Tif)]
        [TestCase("test.jpeg", OutputFormat.Txt)]
        public void HasValidExtension_GivenLotsOfFalseSamples_ReturnsFalse(string filename, OutputFormat outputFormat)
        {
            Assert.IsFalse(_outputFormatHelper.HasValidExtension(filename, outputFormat), $"Sample '{filename}' was deemed valid for {outputFormat}, but should be invalid.");
        }

        [TestCase("test.pdf", OutputFormat.Pdf)]
        [TestCase("test.pdf", OutputFormat.PdfA2B)]
        [TestCase("test.pdf", OutputFormat.PdfX)]
        [TestCase("test.png", OutputFormat.Png)]
        [TestCase("test.tif", OutputFormat.Tif)]
        [TestCase("test.tiff", OutputFormat.Tif)]
        [TestCase("test.TiFf", OutputFormat.Tif)]
        [TestCase("test.jpg", OutputFormat.Jpeg)]
        [TestCase("test.jpeg", OutputFormat.Jpeg)]
        [TestCase("test.jPEG", OutputFormat.Jpeg)]
        [TestCase("test.txt", OutputFormat.Txt)]
        [TestCase(@"c:\MyFolder.Tiff\test.pdf", OutputFormat.Pdf)]
        public void HasValidExtension_GivenLotsOfTrueSamples_ReturnsTrue(string filename, OutputFormat outputFormat)
        {
            Assert.IsTrue(_outputFormatHelper.HasValidExtension(filename, outputFormat), $"Sample '{filename}' was deemed invalid for {outputFormat}, but should be valid.");
        }

        [Test, TestCaseSource(nameof(_outputformatValues))]
        public void IsPdfFormat_ForAllFormats_DoesNotThrowException(OutputFormat outputFormat)
        {
            Assert.DoesNotThrow(() => _outputFormatHelper.IsPdfFormat(outputFormat));
        }

        [TestCase(OutputFormat.Pdf)]
        [TestCase(OutputFormat.PdfA1B)]
        [TestCase(OutputFormat.PdfA2B)]
        [TestCase(OutputFormat.PdfX)]
        public void ForPdfFormats_ReturnsTrue(OutputFormat outputFormat)
        {
            Assert.IsTrue(_outputFormatHelper.IsPdfFormat(outputFormat));
        }

        [TestCase(OutputFormat.Jpeg)]
        [TestCase(OutputFormat.Png)]
        [TestCase(OutputFormat.Tif)]
        [TestCase(OutputFormat.Txt)]
        public void ForOtherFormats_ReturnsFalse(OutputFormat outputFormat)
        {
            Assert.IsFalse(_outputFormatHelper.IsPdfFormat(outputFormat));
        }

        private static OutputFormat[] _outputformatValues = (OutputFormat[])Enum.GetValues(typeof(OutputFormat));
    }
}
