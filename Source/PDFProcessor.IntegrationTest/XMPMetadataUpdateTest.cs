using NUnit.Framework;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFProcessing.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    class XMPMetadataUpdateTest
    {
        private TestHelper _th;
        
        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("PDFProcessing XMP metadata update test");

            _th.GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePdfA);

            //Settings of the previously created outputfile
            _th.Job.JobInfo.Metadata.Title = "Test Title";
            _th.Job.JobInfo.Metadata.Subject = "Test Subject";
            _th.Job.JobInfo.Metadata.Keywords = "Test Keywords";
            _th.Job.JobInfo.Metadata.Author = "Test Author";
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void CheckForXMPMetadataUpdateStrings()
        {
            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
        }
    }
}
