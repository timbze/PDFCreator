using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    public abstract class SigningTestBase
    {
        protected TestHelper TestHelper;
        protected IPdfProcessor PDFProcessor;
        protected TimeServerAccount TimeServerAccount;
        protected Accounts Accounts;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            PDFProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            Accounts = new Accounts();
            TimeServerAccount = new TimeServerAccount();
            Accounts.TimeServerAccounts.Add(TimeServerAccount);

            TestHelper = container.GetInstance<TestHelper>();
            TestHelper.InitTempFolder($"PDFProcessing_{PDFProcessor.GetType().Name}_Signing");

            TestHelper.GenerateGsJob_WithSetOutput(TestFile.ThreePDFCreatorTestpagesPDF);
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            ApplySignatureSettings();
        }

        private void ApplySignatureSettings()
        {
            TestHelper.Job.Accounts = Accounts;
            TestHelper.Job.Profile.PdfSettings.Signature.Enabled = true;

            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFileP12);
            TestHelper.Job.Passwords.PdfSignaturePassword = "Test1";

            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 1;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftX = 50;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftY = 50;
            TestHelper.Job.Profile.PdfSettings.Signature.RightX = 300;
            TestHelper.Job.Profile.PdfSettings.Signature.RightY = 150;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";

            TimeServerAccount.AccountId = "TestAccountId";
            TimeServerAccount.Url = "https://freetsa.org/tsr";
            TimeServerAccount.IsSecured = false;

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerAccountId = TimeServerAccount.AccountId;
        }

        [TearDown]
        public void CleanUp()
        {
            TestHelper.CleanUp();
            FinalizePdfProcessor();
        }

        #region TimeServer

        [Test]
        public void TimeServer_GlobalSign()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TimeServerAccount.Url = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TimeServerAccount.IsSecured = true;
            TimeServerAccount.UserName = "UserName";
            TimeServerAccount.Password = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void TimeServer_DigiCert()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TimeServerAccount.Url = "http://timestamp.digicert.com";

            TimeServerAccount.IsSecured = true;
            TimeServerAccount.UserName = "UserName";
            TimeServerAccount.Password = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void TimeServer_FreeTSA()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;

            TimeServerAccount.IsSecured = true;
            TimeServerAccount.UserName = "UserName";
            TimeServerAccount.Password = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        #endregion TimeServer

        #region PDF

        [Test]
        public void SigningPdf_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TimeServerAccount.Url = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TimeServerAccount.IsSecured = true;
            TimeServerAccount.UserName = "UserName";
            TimeServerAccount.Password = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_CustomPageGreaterThanNumberOfPages()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_CustomPageSpecialCharacters()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_FirstPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_Invisible()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_LastPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdf_UnavailableTimeServer_ThrowsProcessingException()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TimeServerAccount.Url = "http://timestamp.HOPEFULLY.NEVER.EXISTS/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        public void SigningPdf_WrongSignaturePassword_ThrowsProcessingException()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Passwords.PdfSignaturePassword = "Invalid certificate Password";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_WrongCertificatePassword, ex.ErrorCode, "Wrong error code for wrong certificate password");
        }

        [Test]
        [Category("Manual")]
        public void SigningPdf_TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalid()
        {
            if (!Debugger.IsAttached)
                return;

            // TODO make multi-signing testable

            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            Process.Start(multiSignedFile);
            Debugger.Break();
        }

        [Test]
        [Category("Manual")]
        public void SigningPdf_TwoSignatures_MultisigningIsEnabled_FirstSignatureIsValid()
        {
            if (!Debugger.IsAttached)
                return;

            // TODO make multi-signing testable

            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            Process.Start(multiSignedFile);
            Debugger.Break();
        }

        #endregion PDF

        #region PDF/X

        [Test]
        public void SigningPdfX()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_X);
            ApplySignatureSettings();

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        #endregion PDF/X

        #region PDF/A1-b

        [Test]
        public void SigningPdfA1B()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_A_1b);
            ApplySignatureSettings();

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        #endregion PDF/A1-b

        #region PDF/A2-b

        [Test]
        public void SigningPdfA2B()
        {
            TestHelper.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF_A_2b);
            ApplySignatureSettings();

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        #endregion PDF/A2-b

        #region Parallel

        [Test]
        public void Test_Parallel_Signing()
        {
            var tmpfolder = Path.Combine(TestHelper.TmpTestFolder, "output");
            int numTests = 10;

            Directory.CreateDirectory(tmpfolder);
            foreach (var file in Directory.EnumerateFiles(tmpfolder, "*.pdf"))
            {
                File.Delete(file);
            }

            var jobs = new List<Job>();

            for (int i = 0; i < numTests; i++)
            {
                var pdfFile = Path.Combine(tmpfolder, $"{i:D5}.pdf");
                File.Copy(TestHelper.Job.TempOutputFiles[0], pdfFile);

                var job = new Job(new JobInfo(), TestHelper.Job.Profile, new JobTranslations(), Accounts);
                job.Passwords = TestHelper.Job.Passwords;
                job.TempOutputFiles.Add(pdfFile);

                jobs.Add(job);
            }

            Parallel.ForEach(jobs, job =>
            {
                var processor = BuildPdfProcessor();
                processor.ProcessPdf(job);
                job.OutputFiles.Add(job.TempOutputFiles.First());
            });

            jobs.ForEach(SigningTester.TestSignature);
        }

        #endregion Parallel
    }
}
