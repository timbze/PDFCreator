using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFProcessing.IntegrationTest
{
    [TestFixture]
    [Category("LongRunning")]
    internal class SigningTest
    {
        private TestHelper _th;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("PDFProcessing Signing Test");

            _th.GenerateGsJob_WithSettedOutput(TestFile.ThreePDFCreatorTestpagesPDF);
            _th.Job.Passwords.PdfSignaturePassword = "Test1";
            _th.Job.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

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

            try
            {
                PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);
            }
            catch (ProcessingException pEx)
            {
                Assert.AreEqual(12205, pEx.ErrorCode, "Wrong error code for unavailable time server");
                return;
            }

            Assert.Fail("Did not throw ProcessingException for unavailable time server");
        }

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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            SigningTester.TestSignature(_th.Job);
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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);
            
            SigningTester.TestSignature(_th.Job, 2, true);
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

            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);
            _th.Job.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            PDFProcessor.ProcessPDF(_th.Job.OutputFiles[0], _th.Job.Profile, _th.Job.Passwords);

            SigningTester.TestSignature(_th.Job, 2, false);
        }
    }
    
}
