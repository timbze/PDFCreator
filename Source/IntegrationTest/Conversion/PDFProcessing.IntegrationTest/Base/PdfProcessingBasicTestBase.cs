using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    public abstract class PdfProcessingBasicTestBase
    {
        private TestHelper _th;
        private IPdfProcessor _pdfProcessor;

        protected abstract IPdfProcessor BuildPdfProcessor();

        protected abstract void FinalizePdfProcessor();

        [SetUp]
        public void SetUp()
        {
            _pdfProcessor = BuildPdfProcessor();

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder($"PDFProcessing_{_pdfProcessor.GetType().Name}_Basic");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
            FinalizePdfProcessor();
        }

        [Test]
        public void Init_OutputFormatPdfA1b_EncryptionGetsRefused()
        {
            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpagePs);
            _th.Job.Profile = new ConversionProfile();
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            _th.Job.Profile.PdfSettings.Security.Enabled = true;

            _pdfProcessor.Init(_th.Job);

            Assert.IsFalse(_th.Job.Profile.PdfSettings.Security.Enabled);
        }

        [Test]
        public void Init_OutputFormatPdfA1b_DisabledMultiSigningGetsEnabled()
        {
            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpagePs);
            _th.Job.Profile = new ConversionProfile();
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA1B;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.AllowMultiSigning = false;

            _pdfProcessor.Init(_th.Job);

            Assert.IsTrue(_th.Job.Profile.PdfSettings.Signature.AllowMultiSigning);
        }

        [Test]
        public void Init_OutputFormatPdfA2b_EncryptionGetsRefused()
        {
            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpagePs);
            _th.Job.Profile = new ConversionProfile();
            _th.Job.Profile.OutputFormat = OutputFormat.PdfA2B;
            _th.Job.Profile.PdfSettings.Security.Enabled = true;

            _pdfProcessor.Init(_th.Job);

            Assert.IsFalse(_th.Job.Profile.PdfSettings.Security.Enabled);
        }

        [Test]
        public void Init_OutputFormatPdfX_EncryptionGetsRefused()
        {
            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpagePs);
            _th.Job.Profile = new ConversionProfile();
            _th.Job.Profile.OutputFormat = OutputFormat.PdfX;
            _th.Job.Profile.PdfSettings.Security.Enabled = true;

            _pdfProcessor.Init(_th.Job);

            Assert.IsFalse(_th.Job.Profile.PdfSettings.Security.Enabled);
        }

        [Test]
        public void DeterminePdfVersion_DefaultVersionIs1_4()
        {
            var profile = new ConversionProfile();
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.4", version, "Wrong PDFVersion");
        }

        [Test]
        public void DeterminePdfVersion_EnabledSecurityAnd128AesLevel_VersionIs1_6()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Security.Enabled = true;
            profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.6", version, "Wrong PDFVersion");
        }

        [Test]
        public void DeterminePdfVersion_EnabledSigningAndDisabledMultiSigning_VersionIs1_6()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.Enabled = true;
            profile.PdfSettings.Signature.AllowMultiSigning = false;
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.6", version, "Wrong PDFVersion");
        }

        [Test]
        public void DeterminePdfVersion_EnabledSecurityAnd256AesLevel_VersionIs1_7()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Security.Enabled = true;
            profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.7", version, "Wrong PDFVersion");
        }

        [Test]
        public void DeterminePdfVersion_OutputformatIsPdfA1b_VersionIs1_4()
        {
            var profile = new ConversionProfile();
            profile.OutputFormat = OutputFormat.PdfA1B;
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.4", version, "Wrong PDFVersion");
        }

        [Test]
        public void DeterminePdfVersion_OutputformatIsPdfA2b_VersionIs1_7()
        {
            var profile = new ConversionProfile();
            profile.OutputFormat = OutputFormat.PdfA2B;
            var version = _pdfProcessor.DeterminePdfVersion(profile);
            Assert.AreEqual("1.7", version, "Wrong PDFVersion");
        }

        [Test]
        public void CheckIfFileExists_AllPdfPropertiesDisabled()
        {
            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF);
            _th.Job.Profile.OutputFormat = OutputFormat.Pdf;
            //UpdateXMPMetadata disabled if format is not PDF/A
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = false;
            _th.Job.Passwords = new JobPasswords();

            _pdfProcessor.ProcessPdf(_th.Job);

            Assert.IsTrue(File.Exists(_th.Job.OutputFiles[0]), "Processed file does not exist.");
        }

        [Test]
        public void TestingWithJob_CheckIfFileExists_AllPdfPropertiesDisabled()
        {
            _th.Profile.OutputFormat = OutputFormat.Pdf;
            //UpdateXMPMetadata disabled if format is not PDF/A
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = false;
            _th.Profile.BackgroundPage.Enabled = false;

            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF);
            File.Delete(_th.TmpInfFile);
            foreach (string psFile in _th.TmpPsFiles)
                File.Delete(psFile);

            _pdfProcessor.ProcessPdf(_th.Job);

            foreach (string file in _th.Job.OutputFiles)
            {
                Assert.IsTrue(File.Exists(file), "File does not exist after processing: " + file);
                File.Delete(file);
            }
        }

        [Test]
        public void RequireProcessingForPdfA_without_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfA2B);
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPDF_with_Signing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPDF_with_Encryption()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPdf_with_Backgroundpage()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.BackgroundPage.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPDF_with_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPdfX_with_Signing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForPdfX_with_Only_Encryption()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPdfX_with_Signing_And_Encryption()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;  //Encryption is illegal for PDF/X
                                                                  //but the processing should be triggered
            _th.Job.Profile.PdfSettings.Signature.Enabled = true; //because of the Signing
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPdfX_with_Background_And_Encryption()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Security.Enabled = true; //Encryption is illegal for PDF/X
                                                                 //but the processing should be triggered
            _th.Job.Profile.BackgroundPage.Enabled = true;       //because of the Background
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPDFX_with_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Security.Enabled = true; //Encryption is illegal for PDF/X
                                                                 //but the processing should be triggered
            _th.Job.Profile.BackgroundPage.Enabled = true;       //because of the Background
            _th.Job.Profile.PdfSettings.Signature.Enabled = true; //and Singing
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void RequireProcessingForPDF_with_Backgroundpage()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.BackgroundPage.Enabled = true;
            Assert.IsTrue(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForPDF_without_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Security.Enabled = false;
            _th.Job.Profile.BackgroundPage.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForPdfX_with_Encryption_but_without_Backgroundpage_and_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.PdfX);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = false;
            _th.Job.Profile.PdfSettings.Signature.Enabled = false;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForJPEG_with_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Jpeg);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForTif_with_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Tif);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }

        [Test]
        public void DenyProcessingForPng_with_Encryption_Backgroundpage_Singing()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Png);
            _th.Job.Profile.PdfSettings.Security.Enabled = true;
            _th.Job.Profile.BackgroundPage.Enabled = true;
            _th.Job.Profile.PdfSettings.Signature.Enabled = true;
            Assert.IsFalse(_pdfProcessor.ProcessingRequired(_th.Job.Profile));
        }
    }
}
