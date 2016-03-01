using System;
using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Core;
using pdfforge.PDFCreator.Core.Settings.Enums;

namespace PDFCreator.Core.UnitTest
{
    [TestFixture]
    public class OutputFormatHelperTest
    {
        private readonly List<KeyValuePair<string, OutputFormat>> _goodSamples = new List<KeyValuePair<string, OutputFormat>>();
        private readonly List<KeyValuePair<string, OutputFormat>> _badSamples = new List<KeyValuePair<string, OutputFormat>>();

        [SetUp]
        public void PrepareSamples()
        {
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.Pdf));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.PdfA2B));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.PdfX));

            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.Png));

            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.Tif));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.Tif));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.TiFf", OutputFormat.Tif));

            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.Jpeg));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.Jpeg));
            _goodSamples.Add(new KeyValuePair<string, OutputFormat>("test.jPEG", OutputFormat.Jpeg));

            _goodSamples.Add(new KeyValuePair<string, OutputFormat>(@"c:\MyFolder.Tiff\test.pdf", OutputFormat.Pdf));



            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.Pdf));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.PdfA2B));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.PdfX));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.Png));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.Jpeg));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.txt", OutputFormat.Tif));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.Pdf));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.Pdf));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.Pdf));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.Pdf));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.Pdf));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.PdfA2B));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.PdfA2B));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.PdfA2B));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.PdfA2B));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.PdfA2B));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.PdfX));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.PdfX));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.PdfX));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.PdfX));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.PdfX));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.Png));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.Png));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.Png));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.Png));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.Png));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.Jpeg));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tif", OutputFormat.Jpeg));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.tiff", OutputFormat.Jpeg));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.Jpeg));

            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.pdf", OutputFormat.Tif));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.png", OutputFormat.Tif));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpg", OutputFormat.Tif));
            _badSamples.Add(new KeyValuePair<string, OutputFormat>("test.jpeg", OutputFormat.Tif));
        }
        
        [Test]
        public void HasValidExtension_GivenLotsOfTrueSamples_ReturnsTrue()
        {
            foreach (KeyValuePair<string, OutputFormat> sample in _goodSamples)
            {
                Assert.IsTrue(OutputFormatHelper.HasValidExtension(sample.Key, sample.Value), String.Format("Sample '{0}' was deemed invalid for {1}, but should be valid.", sample.Key, sample.Value));
            }
        }

        [Test]
        public void HasValidExtension_GivenLotsOfFalseSamples_ReturnsFalse()
        {

            foreach (KeyValuePair<string, OutputFormat> sample in _badSamples)
            {
                Assert.IsFalse(OutputFormatHelper.HasValidExtension(sample.Key, sample.Value), String.Format("Sample '{0}' was deemed valid for {1}, but should be invalid.", sample.Key, sample.Value));
            }
        }

        [Test]
        public void AddValidExtension_GivenGoodFiles_ReturnsSameFiles()
        {
            foreach (KeyValuePair<string, OutputFormat> sample in _goodSamples)
            {
                Assert.AreEqual(OutputFormatHelper.EnsureValidExtension(sample.Key, sample.Value), sample.Key);
            }
        }

        [Test]
        public void AddValidExtension_GivenBadFiles_ReturnsGoodFiles()
        {
            foreach (KeyValuePair<string, OutputFormat> sample in _badSamples)
            {
                Assert.IsTrue(OutputFormatHelper.HasValidExtension(OutputFormatHelper.EnsureValidExtension(sample.Key, sample.Value), sample.Value));
            }
        }
    }
}
