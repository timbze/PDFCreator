using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest.ConversionTests
{
    [TestFixture]
    [Category("LongRunning")]
    class PdfTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("PDFTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        public void MakePdfTest()
        {
            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf file.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");
            string content = File.ReadAllText(_th.Job.OutputFiles[0], Encoding.Default);
           
            Assert.AreEqual('%', content[0]);
            Assert.AreEqual('P', content[1]);
            Assert.AreEqual('D', content[2]);
            Assert.AreEqual('F', content[3]);
            Assert.AreEqual('-', content[4]);

            Assert.AreEqual('F', content[content.Length - 2]);
            Assert.AreEqual('O', content[content.Length - 3]);
            Assert.AreEqual('E', content[content.Length - 4]);
            Assert.AreEqual('%', content[content.Length - 5]);
            Assert.AreEqual('%', content[content.Length - 6]);

            try
            {
                new iTextSharp.text.pdf.PdfReader(_th.Job.OutputFiles[0]);
            }
            catch (Exception ex)
            {
                Assert.Fail("Invalid PdfFile:" + ex.Message);
            }
        }

        [Test]
        public void PdfRegularTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfRegularCmykTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.ColorModel = ColorModel.Cmyk;
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfA1BTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.PdfA1B);
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfA2BTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.PdfA2B);
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfXTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.PdfX);
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfXGrayTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.ColorModel = ColorModel.Gray;
            _th.RunGsJob();

            MakePdfTest();
        }
    }
}