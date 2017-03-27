using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    [Category("LongRunning")]
    internal abstract class PdfProcessingCombinedTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();
        protected abstract bool IsIText { get; }

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PDFProcessing_IText_Combined");

            _pdfProcessor = BuildPdfProcessor();
        }

        private void GenerateGsJob_WithSettedOutput(TestFile tf)
        {
            _th.GenerateGsJob_WithSetOutput(tf);
            _th.Job.Profile.BackgroundPage.File = _th.GenerateTestFile(TestFile.Background3PagesPDF);
            _th.Job.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);

            _th.Job.Passwords.PdfUserPassword = "User";
            _th.Job.Passwords.PdfOwnerPassword = "Owner";
            _th.Job.Passwords.PdfSignaturePassword = "Test1";

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
        public void EncryptionAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void EncryptionAndSigning()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            _pdfProcessor.ProcessPdf(_th.Job);

            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void EncryptionSigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void MetadataAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.TestpagePDFA2b); //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void MetadataAndSigning()
        {
            GenerateGsJob_WithSettedOutput(TestFile.TestpagePDFA2b); //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            _pdfProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void MetadataSigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.TestpagePDFA2b); //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void SigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void All_EncryptionSigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }
    }
}