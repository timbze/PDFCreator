using System;
using System.Globalization;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Editions.PDFCreator;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    [Category("LongRunning")]
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

        private int CalculateImageByteSize(float width, float height, PdfObject obj, PdfDictionary tg)
        {
            var imgRi = ImageRenderInfo.CreateForXObject(new Matrix(width, height), (PRIndirectReference) obj, tg);
            var length = imgRi.GetImage().GetImageAsBytes().Length;
            return length;
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new int[4];
            var testResultsSize = new int[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = Convert.ToInt32(tg.Get(PdfName.WIDTH).ToString());

                    var height = tg.Get(PdfName.HEIGHT).ToString();
                    testResultsSize[i] = CalculateImageByteSize(testResultsWidth[i], float.Parse(height), obj, tg);
                    i++;
                }
            }

            Assert.AreEqual(null, testResultsFilter[0]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[1]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[2]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[3]);

            Assert.AreEqual(200, testResultsWidth[0]);
            Assert.AreEqual(53, testResultsWidth[1]);
            Assert.AreEqual(53, testResultsWidth[2]);
            Assert.AreEqual(79, testResultsWidth[3]);

            // do asserts with an allowed delta (difference) of 2
            Assert.AreEqual(1995, testResultsSize[0], 2.0);
            Assert.AreEqual(423, testResultsSize[1], 2.0);
            Assert.AreEqual(736, testResultsSize[2], 2.0);
            Assert.AreEqual(661, testResultsSize[3], 2.0);
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new int[4];
            var testResultsSize = new int[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = Convert.ToInt32(tg.Get(PdfName.WIDTH).ToString());

                    var height = tg.Get(PdfName.HEIGHT).ToString();
                    testResultsSize[i] = CalculateImageByteSize(testResultsWidth[i], float.Parse(height), obj, tg);
                    i++;
                }
            }

            Assert.AreEqual(null, testResultsFilter[0]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[1]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[2]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[3]);

            Assert.AreEqual(200, testResultsWidth[0]);
            Assert.AreEqual(200, testResultsWidth[1]);
            Assert.AreEqual(200, testResultsWidth[2]);
            Assert.AreEqual(475, testResultsWidth[3]);

            Assert.AreEqual(1995, testResultsSize[0]);
            Assert.AreEqual(4202, testResultsSize[1]);
            Assert.AreEqual(6179, testResultsSize[2]);
            Assert.AreEqual(4197, testResultsSize[3]);
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new string[4];
            var testResultsSize = new string[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = tg.Get(PdfName.WIDTH).ToString();

                    var height = tg.Get(PdfName.HEIGHT).ToString();
                    testResultsSize[i] = CalculateImageByteSize(float.Parse(testResultsWidth[i]), float.Parse(height), obj, tg)
                        .ToString(CultureInfo.InvariantCulture);
                    i++;
                }
            }

            Assert.AreEqual(null, testResultsFilter[0]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[1]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[2]);
            Assert.AreEqual("/DCTDecode", testResultsFilter[3]);

            Assert.AreEqual("200", testResultsWidth[0]);
            Assert.AreEqual("200", testResultsWidth[1]);
            Assert.AreEqual("200", testResultsWidth[2]);
            Assert.AreEqual("475", testResultsWidth[3]);

            Assert.AreEqual("1995", testResultsSize[0]);
            Assert.AreEqual("17960", testResultsSize[1]);
            Assert.AreEqual("31460", testResultsSize[2]);
            Assert.AreEqual("16899", testResultsSize[3]);
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new int[4];
            var testResultsSize = new int[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = Convert.ToInt32(tg.Get(PdfName.WIDTH).ToString());

                    var height = tg.Get(PdfName.HEIGHT).ToString();

                    testResultsSize[i] = CalculateImageByteSize(testResultsWidth[i], float.Parse(height), obj, tg);

                    i++;
                }
            }

            Assert.AreEqual("/CCITTFaxDecode", testResultsFilter[0]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[1]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[2]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[3]);

            Assert.AreEqual(66, testResultsWidth[0]);
            Assert.AreEqual(53, testResultsWidth[1]);
            Assert.AreEqual(53, testResultsWidth[2]);
            Assert.AreEqual(79, testResultsWidth[3]);

            // do asserts with an allowed delta (difference) of a few bytes
            Assert.AreEqual(448, testResultsSize[0], 30.0);
            Assert.AreEqual(2336, testResultsSize[1], 30.0);
            Assert.AreEqual(6636, testResultsSize[2], 30.0);
            Assert.AreEqual(1017, testResultsSize[3], 30.0);
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new string[4];
            var testResultsSize = new string[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = tg.Get(PdfName.WIDTH).ToString();

                    var height = tg.Get(PdfName.HEIGHT).ToString();

                    var length = CalculateImageByteSize(float.Parse(testResultsWidth[i]), float.Parse(height), obj, tg);

                    testResultsSize[i] = length.ToString(CultureInfo.InvariantCulture);
                    i++;
                }
            }

            Assert.AreEqual("/FlateDecode", testResultsFilter[0]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[1]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[2]);
            Assert.AreEqual("/FlateDecode", testResultsFilter[3]);

            Assert.AreEqual("200", testResultsWidth[0]);
            Assert.AreEqual("200", testResultsWidth[1]);
            Assert.AreEqual("200", testResultsWidth[2]);
            Assert.AreEqual("475", testResultsWidth[3]);

            Assert.AreEqual("1995", testResultsSize[0]);
            Assert.AreEqual("29961", testResultsSize[1]);
            Assert.AreEqual("89669", testResultsSize[2]);
            Assert.AreEqual("11481", testResultsSize[3]);
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

            var pdf = new PdfReader(_th.Job.OutputFiles[0]);

            var pg = pdf.GetPageN(1);
            var res = (PdfDictionary) PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary) PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

            var testResultsFilter = new string[4];
            var testResultsWidth = new string[4];
            var testResultsSize = new string[4];

            var i = 0;
            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    var tg = (PdfDictionary) PdfReader.GetPdfObject(obj);

                    if (tg.Get(PdfName.FILTER) != null)
                        testResultsFilter[i] = tg.Get(PdfName.FILTER).ToString();

                    testResultsWidth[i] = tg.Get(PdfName.WIDTH).ToString();

                    var height = tg.Get(PdfName.HEIGHT).ToString();

                    var length = CalculateImageByteSize(float.Parse(testResultsWidth[i]), float.Parse(height), obj, tg);

                    testResultsSize[i] = length.ToString(CultureInfo.InvariantCulture);
                    i++;
                }
            }

            Assert.AreEqual(null, testResultsFilter[0]);
            Assert.AreEqual(null, testResultsFilter[1]);
            Assert.AreEqual(null, testResultsFilter[2]);
            Assert.AreEqual(null, testResultsFilter[3]);

            Assert.AreEqual("200", testResultsWidth[0]);
            Assert.AreEqual("200", testResultsWidth[1]);
            Assert.AreEqual("200", testResultsWidth[2]);
            Assert.AreEqual("475", testResultsWidth[3]);

            Assert.AreEqual("1995", testResultsSize[0]);
            Assert.AreEqual("29961", testResultsSize[1]);
            Assert.AreEqual("89669", testResultsSize[2]);
            Assert.AreEqual("11481", testResultsSize[3]);
        }
    }
}