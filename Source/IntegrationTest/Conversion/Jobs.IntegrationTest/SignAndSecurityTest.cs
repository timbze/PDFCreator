using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Editions.PDFCreator;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs
{
    [TestFixture]
    [Category("LongRunning")]
    internal class SignAndSecurityTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();

            //override PasswordProvider, because tests write to job.passwords directly and the provider would replace those values with empty strings from the profile
            container.Options.AllowOverridingRegistrations = true;
            container.Register(() => Substitute.For<IProcessingPasswordsProvider>());

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
            TestOnLow40BitEncryption();
            TestOnMedium128BitdEncryption();
            TestOnHigh128AesEncryption();
        }

        private void TestOnLow40BitEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();
            MakeSecurityTest();
            MakeSigningTest();
            MakePasswordTests();
        }

        private void TestOnMedium128BitdEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();
            MakeSecurityTest();
            MakeSigningTest();
            MakePasswordTests();
        }

        private void TestOnHigh128AesEncryption()
        {
            _th.Profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Passwords.PdfSignaturePassword = TestCertPw;
            _th.RunGsJob();
            MakeSecurityTest();
            MakeSigningTest();
            MakePasswordTests();
        }

        private void MakeSecurityTest()
        {
            if (!_th.Profile.PdfSettings.Security.Enabled)
            {
                Assert.That(() =>
                {
                    var reader = new PdfReader(_th.Job.OutputFiles[0]);
                    Assert.AreEqual(PdfWriter.VERSION_1_4, reader.PdfVersion, "Pdf Version is not set to 1.4.");
                    Assert.AreEqual(-1, reader.GetCryptoMode(), "Encryption mode is not -1 (disabled)");
                    Assert.AreEqual(0, reader.Permissions, "Wrong permission value");
                }, !Throws.Exception.TypeOf<BadPasswordException>());
                return;
            }

            var pdfReader = new PdfReader(_th.Job.OutputFiles[0], Encoding.Default.GetBytes(_th.Job.Passwords.PdfOwnerPassword));

            switch (_th.Profile.PdfSettings.Security.EncryptionLevel)
            {
                case EncryptionLevel.Rc40Bit:
                    Assert.AreEqual(PdfWriter.VERSION_1_4, pdfReader.PdfVersion, "Not PDF-Version 1.4 for Low40Bit");
                    Assert.AreEqual(0, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for Low40Bit");
                    break;
                case EncryptionLevel.Rc128Bit:
                    Assert.AreEqual(PdfWriter.VERSION_1_4, pdfReader.PdfVersion, "Not PDF-Version 1.4 for Medium128Bit");
                    Assert.AreEqual(1, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for Medium128Bit");
                    break;
                case EncryptionLevel.Aes128Bit:
                    Assert.AreEqual(PdfWriter.VERSION_1_6, pdfReader.PdfVersion, "Not PDF-Version 1.6 for High128BitAES");
                    Assert.AreEqual(2, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for High128BitAES");
                    break;
            }

            #region check permissions

            long permissionCode = pdfReader.Permissions;
            Assert.AreEqual(-3904, permissionCode & -3904, "Permission-Bit 7-8 and 13-32 are not true! (PDF-Reference)");
            Assert.AreEqual(-4, permissionCode | -4, "Permission-Bit 1-2 are not false! (PDF-Reference)");

            if (_th.Profile.PdfSettings.Security.AllowToCopyContent)
                Assert.AreEqual(PdfWriter.ALLOW_COPY, permissionCode & PdfWriter.ALLOW_COPY,
                    "Requested Allow-Copy is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_COPY,
                    "Unrequested Allow-Copy is set");

            if (_th.Profile.PdfSettings.Security.AllowToEditComments)
                Assert.AreEqual(PdfWriter.ALLOW_MODIFY_ANNOTATIONS, permissionCode & PdfWriter.ALLOW_MODIFY_ANNOTATIONS,
                    "Requested Allow-Modify-Annotations is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_MODIFY_ANNOTATIONS,
                    "Unrequested Allow-Modify-Annotations is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            if (_th.Profile.PdfSettings.Security.AllowToEditTheDocument)
                Assert.AreEqual(PdfWriter.ALLOW_MODIFY_CONTENTS, permissionCode & PdfWriter.ALLOW_MODIFY_CONTENTS,
                    "Requested Allow-Modify-Content is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_MODIFY_CONTENTS,
                    "Unrequested Allow-Modify-Content is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            //Printing
            Assert.IsFalse(!_th.Profile.PdfSettings.Security.AllowPrinting && _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality, "Restrict to degraded printing is set without allowed printing");

            if (_th.Profile.PdfSettings.Security.AllowPrinting && (!_th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality || (_th.Profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit)))
                Assert.AreEqual(PdfWriter.ALLOW_PRINTING, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Requested Allow-Printing is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (!_th.Profile.PdfSettings.Security.AllowPrinting && (_th.Profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(2048, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Requested Allow-Printing is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (!_th.Profile.PdfSettings.Security.AllowPrinting && !_th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality)
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Unrequested Allow-Printing is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (_th.Profile.PdfSettings.Security.AllowPrinting && _th.Profile.PdfSettings.Security.RestrictPrintingToLowQuality)
                Assert.AreEqual(PdfWriter.ALLOW_DEGRADED_PRINTING, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "No Restriction to degraded printing (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption 
            if (_th.Profile.PdfSettings.Security.AllowToEditAssembly || (_th.Profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_ASSEMBLY, permissionCode & PdfWriter.ALLOW_ASSEMBLY,
                    "Requested Allow-Assembly is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_ASSEMBLY,
                    "Unrequested Allow-Assembly is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption 
            if (_th.Profile.PdfSettings.Security.AllowToFillForms || (_th.Profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_FILL_IN, permissionCode & PdfWriter.ALLOW_FILL_IN,
                    "Requested Allow-Fill-In is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_FILL_IN,
                    "Unrequested Allow-Fill-In is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption    
            if (_th.Profile.PdfSettings.Security.AllowScreenReader || (_th.Profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_SCREENREADERS, permissionCode & PdfWriter.ALLOW_SCREENREADERS,
                    "Requested Allow-ScreenReaders is not set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_SCREENREADERS,
                    "Unrequested Allow-ScreenReaders is set (" + _th.Profile.PdfSettings.Security.EncryptionLevel + ")");

            #endregion
        }

        private void MakeSigningTest()
        {
            PdfReader pdfReader;
            if (_th.Profile.PdfSettings.Security.Enabled)
            {
                if (_th.Profile.PdfSettings.Security.RequireUserPassword)
                    pdfReader = new PdfReader(_th.Job.OutputFiles[0], Encoding.Default.GetBytes(_th.Job.Passwords.PdfUserPassword));
                else
                    pdfReader = new PdfReader(_th.Job.OutputFiles[0], Encoding.Default.GetBytes(_th.Job.Passwords.PdfOwnerPassword));
            }
            else
            {
                pdfReader = new PdfReader(_th.Job.OutputFiles[0]);
            }
            var af = pdfReader.AcroFields;

            //Stop here if no Signing was requested 
            if (!_th.Profile.PdfSettings.Signature.Enabled)
            {
                Assert.AreEqual(0, af.GetSignatureNames().Count, "SignatureName(s) in unsingned file.");
                return;
            }
            //Go on to verify the Signature
            Assert.AreEqual(1, af.GetSignatureNames().Count, "Number of SignatureNames must be 1.");

            var signName = af.GetSignatureNames()[0];
            var pk = af.VerifySignature(signName);

            var now = DateTime.UtcNow;
            var ts = now - pk.TimeStampDate;
            Assert.IsTrue(Math.Abs(ts.TotalMinutes) < 30, "Timestamp has a difference, bigger than 1h from now. Now is {0}, signature timestamp: {1}, difference: {2}", now, pk.TimeStampDate, ts);

            Assert.AreEqual(_th.Profile.PdfSettings.Signature.SignLocation, pk.Location, "Wrong SignLocation");
            Assert.AreEqual(_th.Profile.PdfSettings.Signature.SignReason, pk.Reason, "Wrong SignReason");

            if (_th.Profile.PdfSettings.Signature.DisplaySignatureInDocument)
            {
                switch (_th.Profile.PdfSettings.Signature.SignaturePage)
                {
                    case SignaturePage.FirstPage:
                        Assert.AreEqual(1, af.GetFieldPositions(signName)[0].page, "Signature is not on the first page");
                        break;
                    case SignaturePage.CustomPage:
                        if (_th.Profile.PdfSettings.Signature.SignatureCustomPage > pdfReader.NumberOfPages)
                            Assert.AreEqual(pdfReader.NumberOfPages, af.GetFieldPositions(signName)[0].page, "Signature is not on requested page");
                        else
                            Assert.AreEqual(_th.Profile.PdfSettings.Signature.SignatureCustomPage, af.GetFieldPositions(signName)[0].page, "Signature is not on requested page");
                        break;
                    case SignaturePage.LastPage:
                        Assert.AreEqual(pdfReader.NumberOfPages, af.GetFieldPositions(signName)[0].page, "Signature is not on last Page");
                        break;
                }
                Assert.AreEqual(_th.Profile.PdfSettings.Signature.LeftX, (int) af.GetFieldPositions(signName)[0].position.GetLeft(0), "Wrong Position for LeftX");
                Assert.AreEqual(_th.Profile.PdfSettings.Signature.LeftY, (int) af.GetFieldPositions(signName)[0].position.GetBottom(0), "Wrong Position for LeftY");
                Assert.AreEqual(_th.Profile.PdfSettings.Signature.RightX, (int) af.GetFieldPositions(signName)[0].position.GetRight(0), "Wrong Position for RightX");
                Assert.AreEqual(_th.Profile.PdfSettings.Signature.RightY, (int) af.GetFieldPositions(signName)[0].position.GetTop(0), "Wrong Position for RightY");
            }
            else
            {
                Assert.AreEqual(1, af.GetFieldPositions(signName)[0].page, "Wrong position for \"invisible\" signature");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signName)[0].position.GetLeft(0), "Wrong position for \"invisible\" signature");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signName)[0].position.GetBottom(0), "Wrong position for \"invisible\" signature");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signName)[0].position.GetRight(0), "Wrong position for \"invisible\" signature");
                Assert.AreEqual(0, (int) af.GetFieldPositions(signName)[0].position.GetTop(0), "Wrong position for \"invisible\" signature");
            }
        }

        private void MakePasswordTests()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.RunGsJob();
            PasswordTest(true, true);
            PasswordTest("BadPW", true, true);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = false;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            PasswordTest(true, false);
            PasswordTest(_th.Job.Passwords.PdfUserPassword, false, false);
            PasswordTest(_th.Job.Passwords.PdfOwnerPassword, true, true);

            _th.Profile.PdfSettings.Security.Enabled = true;
            _th.Profile.PdfSettings.Security.RequireUserPassword = true;
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _th.Job.Profile.PdfSettings.Signature.SignaturePassword = TestCertPw;
            _th.Job.Passwords.PdfUserPassword = "TestUserPw";
            _th.Job.Passwords.PdfOwnerPassword = "TestOwnerPw";
            _th.RunGsJob();
            PasswordTest(false, false);
            PasswordTest("BadPW", false, false);
            PasswordTest(_th.Job.Passwords.PdfUserPassword, true, false);
            PasswordTest(_th.Job.Passwords.PdfOwnerPassword, true, true);
        }

        /// <summary>
        ///     Test for BadPasswordException while opening and editing pdf file of current testhelper job.
        ///     File gets opend with PDFReader by using the Password (Leave out Password, to open files without encryption).
        /// </summary>
        /// >
        /// <param name="password">Encryption password for pdf file</param>
        /// <param name="canOpen">Set true if opening should be authorized, else false</param>
        /// <param name="canEdit">Set true if editing should be authorized, else false</param>
        private void PasswordTest(string password, bool canOpen, bool canEdit)
        {
            //Testing Opening
            Assert.That(() =>
            {
                var reader = new PdfReader(_th.Job.OutputFiles[0], Encoding.Default.GetBytes(password));
                reader.Close();
            },
                canOpen ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());

            //Testing Editing
            Assert.That(() =>
            {
                var r = new PdfReader(_th.Job.OutputFiles[0], Encoding.Default.GetBytes(password));
                FileStream f;
                using (f = new FileStream(_th.Job.OutputFiles[0] + "StampTest.pdf", FileMode.Create, FileAccess.Write))
                {
                    var s = new PdfStamper(r, f);
                    s.Close();
                    f.Close();
                }
            }, canEdit ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());
        }

        /// <summary>
        ///     Test for BadPasswordException while opening and editing pdf file of current testhelper job.
        ///     File gets opend with PDFReader by using the Password (Leave out Password, to open files without encryption).
        /// </summary>
        /// <param name="canOpen">Set true if opening should be authorized, else false</param>
        /// <param name="canEdit">Set true if editing should be authorized, else false</param>
        private void PasswordTest(bool canOpen, bool canEdit)
        {
            //Testing Opening
            Assert.That(() =>
            {
                var reader = new PdfReader(_th.Job.OutputFiles[0]);
                reader.Close();
            },
                canOpen ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());

            //Testing Editing
            Assert.That(() =>
            {
                var r = new PdfReader(_th.Job.OutputFiles[0]);
                FileStream f;
                using (f = new FileStream(_th.Job.OutputFiles[0] + "StampTest.pdf", FileMode.Create, FileAccess.Write))
                {
                    var s = new PdfStamper(r, f);
                    s.Close();
                    f.Close();
                }
            }, canEdit ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());
        }

        [Test]
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

            MakeSecurityTest();
        }

        [Test]
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
        public void TestSigningCustomPageGreaterThanNumberOfPages()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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
        public void TestSigningCustomPageGreaterThanNumberOfPagesWithCover() //...................................................................
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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
        public void TestSigningCustomPageSpecialCharacters()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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
        public void TestSigningFirstPage()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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
        public void TestSigningInvisible()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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
        public void TestSigningLastPage()
        {
            _th.Profile.PdfSettings.Security.Enabled = false;
            _th.Profile.PdfSettings.Signature.Enabled = true;

            _th.Profile.PdfSettings.Signature.AllowMultiSigning = false; //???
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