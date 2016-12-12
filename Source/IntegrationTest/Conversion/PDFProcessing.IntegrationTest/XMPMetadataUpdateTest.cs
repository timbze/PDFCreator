using SystemWrapper.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Editions.PDFCreator;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing
{
    [TestFixture]
    [Category("LongRunning")]
    internal class XMPMetadataUpdateTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PDFProcessing XMP metadata update test");

            _th.GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePdfA);

            //Settings of the previously created outputfile
            _th.Job.JobInfo.Metadata.Title = "Test Title";
            _th.Job.JobInfo.Metadata.Subject = "Test Subject";
            _th.Job.JobInfo.Metadata.Keywords = "Test Keywords";
            _th.Job.JobInfo.Metadata.Author = "Test Author";

            PDFProcessor = new ITextPdfProcessor(new FileWrap(), new DefaultProcessingPasswordsProvider());
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private ITextPdfProcessor PDFProcessor { get; set; }

        [Test]
        public void CheckForXMPMetadataUpdateStrings()
        {
            PDFProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
        }
    }
}