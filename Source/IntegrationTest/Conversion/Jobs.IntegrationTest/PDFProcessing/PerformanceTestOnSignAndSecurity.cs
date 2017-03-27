using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;
using SimpleInjector;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.PDFProcessing
{
    internal class PerformanceTestOnSignAndSecurityIText : PerformanceTestOnSignAndSecurity
    {
        protected override bool IsIText => true;
        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, ITextPdfProcessor>();
        }
    }

    internal class PerformanceTestOnSignAndSecurityPdfTools : PerformanceTestOnSignAndSecurity
    {
        protected override bool IsIText => false;
        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, PdfToolsPdfProcessor>();
        }
    }

    [TestFixture]
    [Ignore("Trigger manually. Too long execution time for permanent testing.")]
    internal abstract class PerformanceTestOnSignAndSecurity
    {
        protected abstract bool IsIText { get; }
        protected abstract void RegisterPdfProcessor(Container container);
        
        private IPdfProcessor _pdfProcessor;

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();

            //override PasswordProvider, because tests write to job.passwords directly and the provider would replace those values with empty strings from the profile
            container.Options.AllowOverridingRegistrations = true;
            RegisterPdfProcessor(container);
            container.Register(() => Substitute.For<IProcessingPasswordsProvider>());

            _pdfProcessor = container.GetInstance<IPdfProcessor>();

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SigningTest");
            _th.Profile.PdfSettings.Signature.CertificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
        }

        [TearDown]
        public void CleanUp()
        {
            if (_th != null)
                _th.CleanUp();
        }

        private const string TestCertPw = "Test1";
        private TestHelper _th;

        private void TestOnDifferentEncryptionLevels()
        {
            Test_40BitEncryption();
            Test_128BitEncryption();
            Test_128AesEncryption();
            Test_256AesEncryption();
        }

        private void DoAllTheTesting(Job job)
        {
            PdfVersionTester.CheckPDFVersion(job, _pdfProcessor);
            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
            SigningTester.TestSignature(job);
            MakePasswordTests();
        }

        private void Test_40BitEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTheTesting(_th.Job);
        }

        private void Test_128BitEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTheTesting(_th.Job);
        }

        private void Test_128AesEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTheTesting(_th.Job);
        }

        private void Test_256AesEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();

            DoAllTheTesting(_th.Job);
        }

        private void MakePasswordTests()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            EncryptionTester.DoPasswordTest(_th.Job);
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllExtendedPermissions()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowAssembly()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowCopy()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowEverything()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowFillIn()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowModifiyAnnotations()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowModifyContents()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowNoExtendedPermissions()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowNothing()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowPrinting()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestAllowScreenReaders()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestEncryptionEnableFalse()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);

            _th.RunGsJob();

            EncryptionTester.DoSecurityTest(_th.Job, false);
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestRestrictToDegradedPrinting()
        {
            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Signature.Enabled = false;

            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningCustomPageGreaterThanNumberOfPages()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 5;
            _th.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Profile.PdfSettings.Signature.RightX = 200;
            _th.Profile.PdfSettings.Signature.RightY = 40;
            _th.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningCustomPageGreaterThanNumberOfPagesWithCover()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 5;
            _th.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Profile.PdfSettings.Signature.RightX = 200;
            _th.Profile.PdfSettings.Signature.RightY = 40;
            _th.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            _th.Profile.CoverPage.Enabled = true;
            _th.Profile.CoverPage.File = _th.GenerateTestFile(TestFile.Cover2PagesPDF);

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningCustomPageSpecialCharacters()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Profile.PdfSettings.Signature.LeftX = 1000;
            _th.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Profile.PdfSettings.Signature.RightX = 2000;
            _th.Profile.PdfSettings.Signature.RightY = 400;
            _th.Profile.PdfSettings.Signature.SignContact = "^^ Mr.Täst ^^";
            _th.Profile.PdfSettings.Signature.SignLocation = "Tästlènd";
            _th.Profile.PdfSettings.Signature.SignReason = "The Réßön is Tästing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningFirstPage()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.FirstPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Profile.PdfSettings.Signature.LeftX = 300;
            _th.Profile.PdfSettings.Signature.LeftY = 200;
            _th.Profile.PdfSettings.Signature.RightX = 500;
            _th.Profile.PdfSettings.Signature.RightY = 400;
            _th.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningInvisible()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = false;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.CustomPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Profile.PdfSettings.Signature.LeftY = 2;
            _th.Profile.PdfSettings.Signature.RightX = 200;
            _th.Profile.PdfSettings.Signature.RightY = 4;
            _th.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            TestOnDifferentEncryptionLevels();
        }

        [Test]
        [Ignore("Trigger manually. Too long execution time for permanent testing.")]
        public void TestSigningLastPage()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false;
            _th.Profile.PdfSettings.Signature.DisplaySignatureInDocument = true;
            _th.Profile.PdfSettings.Signature.SignaturePage = SignaturePage.LastPage;
            _th.Profile.PdfSettings.Signature.SignatureCustomPage = 2;
            _th.Profile.PdfSettings.Signature.LeftX = 100;
            _th.Profile.PdfSettings.Signature.LeftY = 20;
            _th.Profile.PdfSettings.Signature.RightX = 200;
            _th.Profile.PdfSettings.Signature.RightY = 40;
            _th.Profile.PdfSettings.Signature.SignContact = "Mr.Test";
            _th.Profile.PdfSettings.Signature.SignLocation = "Testland";
            _th.Profile.PdfSettings.Signature.SignReason = "The Reason is Testing";
            _th.Profile.PdfSettings.Signature.TimeServerUrl = "http://timestamp.globalsign.com/scripts/timestamp.dll";

            _th.Profile.PdfSettings.Security.Enabled = false;

            TestOnDifferentEncryptionLevels();
        }
    }
}