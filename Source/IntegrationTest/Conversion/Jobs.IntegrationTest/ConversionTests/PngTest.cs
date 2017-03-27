using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    [Category("LongRunning")]
    internal class PngTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PngTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)
            var fs = File.OpenRead(fullFilePath);
            try
            {
                var bytes = new byte[32];
                fs.Read(bytes, 0, Convert.ToInt32(32));
                return bytes;
            }
            finally
            {
                fs.Close();
            }
        }

        public void MakePngTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            _th.RunGsJob();

            foreach (var file in _th.Job.OutputFiles)
            {
                var bytes = GetBytesFromFile(file);

                //check if png
                Assert.AreEqual(137, bytes[0], "No PNG-Form: 1st Byte is not 137");
                Assert.AreEqual(80, bytes[1], "No PNG-Form: 2nd Byte is not P");
                Assert.AreEqual(78, bytes[2], "No PNG-Form: 3rd Byte is not N");
                Assert.AreEqual(71, bytes[3], "No PNG-Form: 3rd Byte is not G");

                switch (_th.Profile.PngSettings.Color)
                {
                    case PngColor.BlackWhite:
                        Assert.AreEqual(0, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(2, bytes[24]*2, "wrong color-scheme according to byte[24-25]");
                        break;
                    case PngColor.Gray8Bit:
                        Assert.AreEqual(0, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(8, bytes[24], "wrong color-scheme according to byte[24-25]");
                        break;
                    case PngColor.Color32BitTransp:
                        Assert.AreEqual(6, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(32, bytes[24]*4, "wrong color-scheme according to byte[24-25]");
                        break;
                    case PngColor.Color24Bit:
                        Assert.AreEqual(2, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(24, bytes[24]*3, "wrong color-scheme according to byte[24-25]");
                        break;
                    case PngColor.Color8Bit:
                        Assert.AreEqual(3, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(8, bytes[24], "wrong color-scheme according to byte[24-25]");
                        break;
                    case PngColor.Color4Bit:
                        Assert.AreEqual(3, bytes[25], "wrong color-scheme according to byte[24-25]");
                        Assert.AreEqual(4, bytes[24], "wrong color-scheme according to byte[24-25]");
                        break;
                }

                var img = Image.FromFile(file);
                Assert.AreEqual(img.HorizontalResolution, img.VerticalResolution, "vertical resolution != horizontal resolution");
                Assert.AreEqual(_th.Profile.PngSettings.Dpi, img.VerticalResolution, 0.013, "vertical resolution != choosen dpi");
                img.Dispose();
            }
        }

        [Test]
        public void TestBlackWhite2Bit()
        {
            _th.Profile.PngSettings.Color = PngColor.BlackWhite;
            _th.Profile.PngSettings.Dpi = 106;
            MakePngTest();
        }

        [Test]
        public void TestColor24Bit()
        {
            _th.Profile.PngSettings.Color = PngColor.Color24Bit;
            _th.Profile.PngSettings.Dpi = 502;
            MakePngTest();
        }

        //one test for each colour-scheme, each with different resolution
        [Test]
        public void TestColor32BitTransp()
        {
            _th.Profile.PngSettings.Color = PngColor.Color32BitTransp;
            _th.Profile.PngSettings.Dpi = 601;
            MakePngTest();
        }

        [Test]
        public void TestColor4Bit()
        {
            _th.Profile.PngSettings.Color = PngColor.Color4Bit;
            _th.Profile.PngSettings.Dpi = 304;
            MakePngTest();
        }

        [Test]
        public void TestColor8Bit()
        {
            _th.Profile.PngSettings.Color = PngColor.Color8Bit;
            _th.Profile.PngSettings.Dpi = 403;
            MakePngTest();
        }

        [Test]
        public void TestGray8Bit()
        {
            _th.Profile.PngSettings.Color = PngColor.Gray8Bit;
            _th.Profile.PngSettings.Dpi = 205;
            MakePngTest();
        }
    }
}