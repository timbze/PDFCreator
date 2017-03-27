using System.Diagnostics;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    [Category("LongRunning")]
    internal abstract class SigningTestBase
    {
        protected TestHelper TestHelper;
        protected IPdfProcessor PDFProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            TestHelper = container.GetInstance<TestHelper>();
            TestHelper.InitTempFolder("PDFProcessing_IText_Signing");

            TestHelper.GenerateGsJob_WithSetOutput(TestFile.ThreePDFCreatorTestpagesPDF);
            TestHelper.Job.Passwords.PdfSignaturePassword = "Test1";
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFileP12);
            TestHelper.Job.Profile.PdfSettings.Signature.Enabled = true;

            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
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
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PDFProcessor = BuildPdfProcessor();
        }

        [TearDown]
        public void CleanUp()
        {
            TestHelper.CleanUp();
        }

        #region PDF

        [Test]
        public void SigningPdf_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerIsSecured = true;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerLoginName = "TimeServerLoginName";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerPassword = "TimeServerPassword";

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
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

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
        public void SigningPdf_UnavailableTimeServer()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.123456.hopefully.never.exists.123456.globalsign.com/scripts/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdf_TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalidLastSignatureIsValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdf_TwoSignatures_MultisigningIsEnabled_BothSignatuesAreValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        #endregion

        #region PDF/X

        [Test]
        public void SigningPdfX_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerIsSecured = true;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerLoginName = "TimeServerLoginName";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerPassword = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_CustomPageGreaterThanNumberOfPages()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_CustomPageSpecialCharacters()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_FirstPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_Invisible()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_LastPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfX_UnavailableTimeServer()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.hopefully.never.exists.123456.globalsign.com/scripts/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfX_TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalidLastSignatureIsValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            Process.Start(multiSignedFile);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfX_TwoSignatures_MultisigningIsEnabled_BothSignatuesAreValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        #endregion

        #region PDF/A1-b

        [Test]
        public void SigningPdfA1B_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerIsSecured = true;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerLoginName = "TimeServerLoginName";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerPassword = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_CustomPageGreaterThanNumberOfPages()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_CustomPageSpecialCharacters()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_FirstPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_Invisible()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_LastPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA1B_UnavailableTimeServer()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.123456.hopefully.never.exists.123456.globalsign.com/scripts/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfA1b_TwoSignatures_MultisigningIsDisabled_BothSignatureAreValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);

            //Enable AllowMultiSigning for Test
            //PDF/A-1b does not support blocking signatures
            //and should automatically enable AllowMultiSigning
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfA1b_TwoSignatures_MultisigningIsEnabled_BothSignatuesAreValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        #endregion

        #region PDF/A2-b

        [Test]
        public void SigningPdfA2B_SecuredTimeServerEnabled_TimeserverDoesNotRequireLogin()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerIsSecured = true;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerLoginName = "TimeServerLoginName";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerPassword = "TimeServerPassword";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_CustomPageGreaterThanNumberOfPages()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 5;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_CustomPageSpecialCharacters()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_FirstPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_Invisible()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_LastPage()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            SigningTester.TestSignature(TestHelper.Job);
            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
        }

        [Test]
        public void SigningPdfA2B_UnavailableTimeServer()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.123456.hopefully.never.exists.123456.globalsign.com/scripts/timestamp.dll";

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_NoTimeServerConnection, ex.ErrorCode, "Wrong error code for unavailable time server");
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfA2b_TwoSignatures_MultisigningIsDisabled_FirstSignatureIsInvalidLastSignatureIsValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftX = 300;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftY = 200;
            TestHelper.Job.Profile.PdfSettings.Signature.RightX = 500;
            TestHelper.Job.Profile.PdfSettings.Signature.RightY = 400;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";
            TestHelper.Job.Profile.PdfSettings.Security.Enabled = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        [Test]
        [Ignore("Currently not possible to detect signatures broken by multisigning. Debug manually to view resultig file")]
        public void SigningPdfA2b_TwoSignatures_MultisigningIsEnabled_BothSignatuesAreValid()
        {
            TestHelper.Job.Profile.PdfSettings.Signature.AllowMultiSigning = true;

            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            TestHelper.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            TestHelper.Job.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftX = 300;
            TestHelper.Job.Profile.PdfSettings.Signature.LeftY = 200;
            TestHelper.Job.Profile.PdfSettings.Signature.RightX = 500;
            TestHelper.Job.Profile.PdfSettings.Signature.RightY = 400;
            TestHelper.Job.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            TestHelper.Job.Profile.PdfSettings.Signature.SignLocation = "Testland";
            TestHelper.Job.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            TestHelper.Job.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";
            TestHelper.Job.Profile.PdfSettings.Security.Enabled = false;

            PDFProcessor.ProcessPdf(TestHelper.Job);

            PdfVersionTester.CheckPDFVersion(TestHelper.Job, PDFProcessor);
            SigningTester.TestSignature(TestHelper.Job);
            var multiSignedFile = SigningTester.TestMultipleSigning(TestHelper.Job, PDFProcessor);

            if (Debugger.IsAttached)
            {
                Process.Start(multiSignedFile);
                Debugger.Break();
            }
        }

        #endregion
    }
}