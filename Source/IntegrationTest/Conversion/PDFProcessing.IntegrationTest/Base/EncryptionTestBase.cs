using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.Base
{
    [TestFixture]
    [Category("LongRunning")]
    internal abstract class EncryptionTestBase
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
            _th.InitTempFolder("PDFProcessing_IText_Encryption");

            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpagePDF);
            _th.Job.Passwords.PdfOwnerPassword = "Owner";
            _th.Job.Passwords.PdfUserPassword = "User";

            _pdfProcessor = BuildPdfProcessor();
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private void TestEncryption()
        {
            _pdfProcessor.ProcessPdf(_th.Job);

            PdfVersionTester.CheckPDFVersion(_th.Job, _pdfProcessor);
            EncryptionTester.DoPasswordTest(_th.Job);
            EncryptionTester.DoSecurityTest(_th.Job, IsIText);
        }

        [Test]
        public void AllowAllExtendedPermissions_NoUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_NoUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_NoUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_NoUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_RequireUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_RequireUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_RequireUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowAllExtendedPermissions_RequireUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_NoUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_NoUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_NoUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_NoUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_RequireUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_RequireUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_RequireUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowEverything_RequireUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = true;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = true;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = true;
            _th.Profile.PdfSettings.Security.AllowScreenReader = true;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_NoUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_NoUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_NoUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_NoUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_RequireUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_RequireUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_RequireUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNoExtendedPermissions_RequireUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = true;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = true;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = true;
            _th.Profile.PdfSettings.Security.AllowPrinting = true;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_NoUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_NoUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_NoUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_NoUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_RequireUserPw_Aes256Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_RequireUserPw_Aes128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_RequireUserPw_Rc128Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void AllowNothing_RequireUserPw_Rc40Bit()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;

            _th.Profile.PdfSettings.Security.AllowToEditAssembly = false;
            _th.Profile.PdfSettings.Security.AllowToEditComments = false;
            _th.Profile.PdfSettings.Security.AllowToCopyContent = false;
            _th.Profile.PdfSettings.Security.AllowToFillForms = false;
            _th.Profile.PdfSettings.Security.AllowToEditTheDocument = false;
            _th.Profile.PdfSettings.Security.AllowPrinting = false;
            _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality = false;
            _th.Profile.PdfSettings.Security.AllowScreenReader = false;

            TestEncryption();
        }

        [Test]
        public void NoEncryption()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;

            TestEncryption();
        }
    }
}