using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    class XmpMetadataUpdateTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("XMPMetadataUpdateTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void CheckForXMPMetadataUpdateStrings_Neu()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA2B);

            _th.Job.JobInfo.Metadata.Title = "Test Title";
            _th.Job.JobInfo.Metadata.Subject = "Test Subject";
            _th.Job.JobInfo.Metadata.Keywords = "Test Keywords";
            _th.Job.JobInfo.Metadata.Author = "Test Author";

            _th.RunGsJob();

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);

        }
    }
}

