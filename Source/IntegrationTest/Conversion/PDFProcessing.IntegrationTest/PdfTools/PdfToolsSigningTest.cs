using SystemWrapper.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    class PdfToolsSigningTest : SigningTestBase
    {
        protected override IPdfProcessor BuildPdfProcessor()
        {
            var pdfToolsLicensing = new PdfToolsTestLicensing();
            Assert.IsTrue(pdfToolsLicensing.Apply(), "Could not apply pdf-tools licensing.");

            return new PdfToolsPdfProcessor(new FileWrap(), new DefaultProcessingPasswordsProvider());
        }

        [Test]
        public void SigningPdf_ExpiredLicense()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.Pdf;
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFile_ExpiredP12);

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_Invalid, ex.ErrorCode, "Wrong error code for expired certificate/signature");
        }

        [Test]
        public void SigningPdfA1B_ExpiredLicense()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFile_ExpiredP12);

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_Invalid, ex.ErrorCode, "Wrong error code for expired certificate/signature");
        }

        [Test]
        public void SigningPdfA2B_ExpiredLicense()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFile_ExpiredP12);

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_Invalid, ex.ErrorCode, "Wrong error code for expired certificate/signature");
        }

        [Test]
        public void SigningPdfX_ExpiredLicense()
        {
            TestHelper.Job.Profile.OutputFormat = OutputFormat.PdfX;
            TestHelper.Job.Profile.PdfSettings.Signature.CertificateFile = TestHelper.GenerateTestFile(TestFile.CertificationFile_ExpiredP12);

            var ex = Assert.Throws<ProcessingException>(() => PDFProcessor.ProcessPdf(TestHelper.Job));
            Assert.AreEqual(ErrorCode.Signature_Invalid, ex.ErrorCode, "Wrong error code for expired certificate/signature");
        }
    }
}
