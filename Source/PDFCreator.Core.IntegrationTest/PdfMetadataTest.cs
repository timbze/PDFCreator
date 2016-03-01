using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    class PdfMetadataTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("MetadataTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void TestMetadata()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);

            _th.Job.JobInfo.Metadata.Title = "Title with äüößéèê€";
            _th.Job.JobInfo.Metadata.Subject = "Subject @";
            _th.Job.JobInfo.Metadata.Keywords = "";
            _th.Job.JobInfo.Metadata.Author = "Author";

            _th.RunGsJob();
                     
            PdfReader pdf = new PdfReader(_th.Job.OutputFiles[0]);
            
            Assert.AreEqual(_th.Job.JobInfo.Metadata.Title, pdf.Info["Title"], "Wrong Title in Metadata");
            Assert.AreEqual(_th.Job.JobInfo.Metadata.Subject, pdf.Info["Subject"], "Wrong Subject in Metadata");
            Assert.AreEqual(_th.Job.JobInfo.Metadata.Keywords, pdf.Info["Keywords"], "Wrong Keywords in Metadata");
            Assert.AreEqual(_th.Job.JobInfo.Metadata.Author, pdf.Info["Author"], "Wrong Author in Metadata");
            Assert.AreEqual(_th.Job.JobInfo.Metadata.Producer, pdf.Info["Producer"], "Wrong Producer in Metadata");  
        }
    }
}
