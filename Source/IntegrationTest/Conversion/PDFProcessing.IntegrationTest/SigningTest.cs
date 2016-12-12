using SystemWrapper.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Editions.PDFCreator;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing
{
    [TestFixture]
    [Category("LongRunning")]
    internal class SigningTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PDFProcessing Signing Test");

            _th.GenerateGsJob_WithSettedOutput(TestFile.ThreePDFCreatorTestpagesPDF);
            _th.Job.Passwords.PdfSignaturePassword = "Test1";
            _th.Job.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;

            PdfProcessor = new ITextPdfProcessor(new FileWrap(), new DefaultProcessingPasswordsProvider());
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private ITextPdfProcessor PdfProcessor { get; set; }

        [Test]
        public void TestSigning_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 1;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Job.Profile.PdfSettings.Signature.RightX = 200;
            _th.Job.Profile.PdfSettings.Signature.RightY = 40;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Job.Profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _th.Job.Profile.PdfSettings.Signature.TimeServerLoginName = "TimeServerLoginName";
            _th.Job.Profile.PdfSettings.Signature.TimeServerPassword = "TimeServerPassword";

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestSigningCustomPageGreaterThanNumberOfPages()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Job.Profile.PdfSettings.Signature.RightX = 200;
            _th.Job.Profile.PdfSettings.Signature.RightY = 40;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestSigningCustomPageSpecialCharacters()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 1000;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Job.Profile.PdfSettings.Signature.RightX = 2000;
            _th.Job.Profile.PdfSettings.Signature.RightY = 400;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestSigningFirstPage()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 300;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Job.Profile.PdfSettings.Signature.RightX = 500;
            _th.Job.Profile.PdfSettings.Signature.RightY = 400;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Job.Profile.PdfSettings.Security.Enabled = false;

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestSigningInvisible()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 2;
            _th.Job.Profile.PdfSettings.Signature.RightX = 200;
            _th.Job.Profile.PdfSettings.Signature.RightY = 4;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestSigningLastPage()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Job.Profile.PdfSettings.Signature.RightX = 200;
            _th.Job.Profile.PdfSettings.Signature.RightY = 40;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job);
        }

        [Test]
        public void TestUnavailableTimeServer()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Job.Profile.PdfSettings.Signature.RightX = 200;
            _th.Job.Profile.PdfSettings.Signature.RightY = 40;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.123456.hopefully.never.exists.123456.globalsign.com/scripts/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PdfProcessor.ProcessPdf(_th.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        public void TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalidLastSignatureIsValid()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 300;

            _th.Job.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Job.Profile.PdfSettings.Signature.RightX = 500;
            _th.Job.Profile.PdfSettings.Signature.RightY = 400;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";
            _th.Job.Profile.PdfSettings.Security.Enabled = false;

            PdfProcessor.ProcessPdf(_th.Job);
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job, 2, false);
        }

        //*
        [Test]
        public void TwoSignatures_MultisigningIsEnabled_BothSignatuesAreValid()
        {
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            _th.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            _th.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Job.Profile.PdfSettings.Signature.LeftX = 300;
            _th.Job.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Job.Profile.PdfSettings.Signature.RightX = 500;
            _th.Job.Profile.PdfSettings.Signature.RightY = 400;
            _th.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";
            _th.Job.Profile.PdfSettings.Security.Enabled = false;

            PdfProcessor.ProcessPdf(_th.Job);
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            PdfProcessor.ProcessPdf(_th.Job);

            SigningTester.TestSignature(_th.Job, 2, true);
        }
    }
}