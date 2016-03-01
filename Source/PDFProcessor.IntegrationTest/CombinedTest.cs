using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFProcessing.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    class CombinedTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("PDFProcessing combined test");
            _th.GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpagePdfA);

            _th.Job.Profile.BackgroundPage.File = _th.GenerateTestFile(TestFile.Background3PagesPDF);
            _th.Job.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);

            _th.Job.Passwords.PdfUserPassword = "User";
            _th.Job.Passwords.PdfOwnerPassword = "Owner";
            _th.Job.Passwords.PdfSignaturePassword = "Test1";

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
        public void All_MatadataUpdateEncryptionSigningAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;  

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            EncryptionTester.MakeSecurityTest(_th.Job);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void MetadataAndEncryption()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            EncryptionTester.MakeSecurityTest(_th.Job);
        }

        [Test]
        public void MetadataAndSigning()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void EncryptionAndSigning()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.Pdf; //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            EncryptionTester.MakeSecurityTest(_th.Job);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void MetadataEncryptionAndSigning()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            EncryptionTester.MakeSecurityTest(_th.Job);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void MetadataAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void EncryptionAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.Pdf; //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            EncryptionTester.MakeSecurityTest(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void MetadataEncryptionAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            EncryptionTester.MakeSecurityTest(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void SigningAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.Pdf; //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void MetadataSigningAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B; //Enables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            XmpMetadataTester.CheckForXMPMetadataUpdateStrings(_th.Job);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void EncryptionSigningAndBackground()
        {
            _th.Job.Profile.OutputFormat = OutputFormat.Pdf; //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            EncryptionTester.MakeSecurityTest(_th.Job);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }
    }
}