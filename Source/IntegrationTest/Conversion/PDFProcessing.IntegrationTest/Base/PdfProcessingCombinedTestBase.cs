using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    internal abstract class PdfProcessingCombinedTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        protected abstract bool IsIText { get; }

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_Combined");
        }

        private void GenerateGsJob_WithSettedOutput(TestFile tf)
        {
            _th.GenerateGsJob_WithSetOutput(tf);

            var accounts = new Accounts();
            var timeServerAccount = new TimeServerAccount();
            timeServerAccount.AccountId = "ExistingTimerServerAccountId";
            accounts.TimeServerAccounts.Add(timeServerAccount);
            _th.Job.Accounts = accounts;
            _th.Job.Profile.PdfSettings.Signature.TimeServerAccountId = timeServerAccount.AccountId;

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
            FinalizePdfProcessor();
        }

        [Test]
        public void EncryptionSigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);
            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void EncryptionAndSigning()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;

            _pdfProcessor.ProcessPdf(_th.Job);

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);
            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void EncryptionAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);
            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }

        [Test]
        public void SigningAndBackground()
        {
            GenerateGsJob_WithSettedOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF); //Disables pdf metadata update
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;

            _pdfProcessor.ProcessPdf(_th.Job);

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);
            SigningTester.TestSignature(_th.Job);
            BackgroundPageTester.BackgroundOnPage(_th.Job);
        }
    }
}
