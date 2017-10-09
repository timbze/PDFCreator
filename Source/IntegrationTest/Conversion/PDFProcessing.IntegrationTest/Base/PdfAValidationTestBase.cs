using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    internal abstract class PdfAValidationTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            container.Options.AllowOverridingRegistrations = true;
            container.Register(() => _pdfProcessor);
            container.Options.AllowOverridingRegistrations = false;

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_PDFA");
        }

        private void InitializeTest(OutputFormat outputFormat)
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, outputFormat);

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
        public void ValidatePdfA1B()
        {
            InitializeTest(OutputFormat.PdfA1B);
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }

        [Test]
        public void ValidatePdfA2B()
        {
            InitializeTest(OutputFormat.PdfA2B);
            _th.RunGsJob();
            PDFValidation.ValidatePdf(_th.Job);
        }
    }
}
