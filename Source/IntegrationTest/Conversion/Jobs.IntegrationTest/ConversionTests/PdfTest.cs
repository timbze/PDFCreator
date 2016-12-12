using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    [Category("LongRunning")]
    internal class PdfTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PDFTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        public void MakePdfTest()
        {
            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Job created more or less than one pdf file.");
            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Outputfile does not exist");
            var content = File.ReadAllText(_th.Job.OutputFiles[0], Encoding.Default);

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
                new PdfReader(_th.Job.OutputFiles[0]);
            }
            catch (Exception ex)
            {
                Assert.Fail("Invalid PdfFile:" + ex.Message);
            }
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
        public void PdfRegularCmykTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.ColorModel = ColorModel.Cmyk;
            _th.RunGsJob();

            MakePdfTest();
        }

        [Test]
        public void PdfRegularTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
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

        [Test]
        public void PdfXTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.PdfX);
            _th.RunGsJob();

            MakePdfTest();
        }
    }
}