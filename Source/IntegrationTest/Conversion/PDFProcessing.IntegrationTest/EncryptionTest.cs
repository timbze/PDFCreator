using SystemWrapper.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Editions.PDFCreator;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing
{
    [TestFixture]
    [Category("LongRunning")]
    internal class EncryptionTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PDFProcessing encryption test");

            _testFile = _th.GenerateTestFile(TestFile.PDFCreatorTestpagePDF);
            _passwords = new JobPasswords();
            _passwords.PdfOwnerPassword = "Owner";
            _passwords.PdfUserPassword = "User";

            PdfProcessor = new ITextPdfProcessor(new FileWrap(), new DefaultProcessingPasswordsProvider());
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private string _testFile;
        private JobPasswords _passwords;

        private TestHelper _th;

        private ITextPdfProcessor PdfProcessor { get; set; }

        private void TestEncryption()
        {
            var job = new Job(new JobInfo(), _th.Profile, new JobTranslations(), new Accounts());
            job.Passwords = _passwords;
            job.TempOutputFiles.Add(_testFile);

            PdfProcessor.ProcessPdf(job);

            EncryptionTester.MakePasswordTest(_testFile, _th.Profile, _passwords);
            EncryptionTester.MakeSecurityTest(_testFile, _th.Profile, _passwords.PdfOwnerPassword);
        }

        [Test]
        public void AllowAllExtendedPermissions_NoUserPw_High128BitAes()
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
        public void AllowAllExtendedPermissions_NoUserPw_Low40Bit()
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
        public void AllowAllExtendedPermissions_NoUserPw_Medium128Bit()
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
        public void AllowAllExtendedPermissions_RequireUserPw_High128BitAes()
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
        public void AllowAllExtendedPermissions_RequireUserPw_Low40Bit()
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
        public void AllowAllExtendedPermissions_RequireUserPw_Medium128Bit()
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
        public void AllowEverything_NoUserPw_High128BitAes()
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
        public void AllowEverything_NoUserPw_Low40Bit()
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
        public void AllowEverything_NoUserPw_Medium128Bit()
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
        public void AllowEverything_RequireUserPw_High128BitAes()
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
        public void AllowEverything_RequireUserPw_Low40Bit()
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
        public void AllowEverything_RequireUserPw_Medium128Bit()
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
        public void AllowNoExtendedPermissions_NoUserPw_High128BitAes()
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
        public void AllowNoExtendedPermissions_NoUserPw_Low40Bit()
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
        public void AllowNoExtendedPermissions_NoUserPw_Medium128Bit()
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
        public void AllowNoExtendedPermissions_RequireUserPw_High128BitAes()
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
        public void AllowNoExtendedPermissions_RequireUserPw_Low40Bit()
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
        public void AllowNoExtendedPermissions_RequireUserPw_Medium128Bit()
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
        public void AllowNothing_NoUserPw_High128BitAes()
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
        public void AllowNothing_NoUserPw_Low40Bit()
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
        public void AllowNothing_NoUserPw_Medium128Bit()
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
        public void AllowNothing_RequireUserPw_High128BitAes()
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
        public void AllowNothing_RequireUserPw_Low40Bit()
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
        public void AllowNothing_RequireUserPw_Medium128Bit()
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
        public void NoEncryption()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;

            TestEncryption();
        }
    }
}