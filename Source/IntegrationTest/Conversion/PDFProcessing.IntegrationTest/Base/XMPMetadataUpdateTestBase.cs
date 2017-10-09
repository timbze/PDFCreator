using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    [Category("LongRunning")]
    internal abstract class XMPMetadataUpdateTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();
        protected abstract void FinalizePdfProcessor();

        private void InitializeTest(TestFile tf)
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_XMPMetadata");
            _th.GenerateGsJob_WithSetOutput(tf);

            //Settings of the set outputfile
            _th.Job.JobInfo.Metadata.Title = "Test Title";
            _th.Job.JobInfo.Metadata.Subject = "Test Subject";
            _th.Job.JobInfo.Metadata.Keywords = "Test Keywords";
            _th.Job.JobInfo.Metadata.Author = "Test Author";
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
            FinalizePdfProcessor();
        }

        [Test]
        public void CheckForXMPMetadataUpdateStrings_PDFA1b()
        {
            InitializeTest(TestFile.TestpagePDFA1b);

            _pdfProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
        }

        [Test]
        public void CheckForXMPMetadataUpdateStrings_PDFA2b()
        {
            InitializeTest(TestFile.TestpagePDFA2b);

            _pdfProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
        }
    }
}