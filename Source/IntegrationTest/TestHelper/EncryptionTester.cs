using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.IO;
using System.Text;

namespace PDFCreator.TestUtilities
{
    public static class EncryptionTester
    {
        /// <summary>
        ///     Test if the testfile can be opened and/or edited according to the security settings in the profile.
        ///     Test is done with the given job.Passwords, without password and with a bad password.
        /// </summary>
        public static void DoPasswordTest(Job job)
        {
            var profile = job.Profile;
            var testFile = job.OutputFiles[0];
            var passwords = job.Passwords;

            var askUserPw = profile.PdfSettings.Security.Enabled && profile.PdfSettings.Security.RequireUserPassword;

            //Owner password
            //File can always be opened and edited with owner password.
            PasswordTest(testFile, true, true, passwords.PdfOwnerPassword);

            //User password
            //File can only be opened with user password is user pw is required.
            //Note: If encryption is disabled, the files can be opened with PdfReader using any password.
            //      If encryption is enabled, it is not possible, although no password is required.
            //File can be edited with user password if no owner password is requested (respectively security is disabled).
            PasswordTest(testFile, askUserPw || !profile.PdfSettings.Security.Enabled, !profile.PdfSettings.Security.Enabled, passwords.PdfUserPassword);

            //Without password
            //File can be opened without password if no user password is requested (or security is disabled (of course...)).
            //File can only be opened with a bad password if user pw is required.
            //Note: If encryption is disabled, the files can be opened with PdfReader using any password.
            //      If encryption is enabled, it is not possible, although no password is required.
            //File can be edited without (or bad) password if no owner password is requested (respectively security is disabled).
            PasswordTest(testFile, !askUserPw, !profile.PdfSettings.Security.Enabled);
            PasswordTest(testFile, !profile.PdfSettings.Security.Enabled, !profile.PdfSettings.Security.Enabled, "BadPassword" + DateTime.Now.Millisecond);
        }

        /// <summary>
        ///     Test for BadPasswordException while opening and editing pdf file of current testhelper job.
        ///     File gets opend with PDFReader by using the Password (Leave out Password, to open test without Password).
        /// </summary>
        /// <param name="testFile">PDF test file</param>
        /// <param name="canOpen">Set true if opening should be authorized, else false</param>
        /// <param name="canEdit">Set true if editing should be authorized, else false</param>
        private static void PasswordTest(string testFile, bool canOpen, bool canEdit)
        {
            //Testing Opening
            Assert.That(() =>
            {
                using (new PdfReader(testFile))
                {
                }
            },
                canOpen ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());

            //Testing Editing
            Assert.That(() =>
            {
                using (var reader = new PdfReader(testFile))
                {
                    var file = testFile + "PasswordTestCopy.pdf";
                    using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        var s = new PdfStamper(reader, fileStream);
                        s.Close();
                        fileStream.Close();
                    }
                    File.Delete(file);
                }
            }, canEdit ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());
        }

        /// <summary>
        ///     Test for BadPasswordException while opening and editing pdf file of current testhelper job.
        ///     File gets opened with PDFReader by using the Password (Leave out Password, to test without Password).
        /// </summary>
        /// >
        /// <param name="testFile">PDF test file</param>
        /// <param name="canOpen">Set true if opening should be authorized, else false</param>
        /// <param name="canEdit">Set true if editing should be authorized, else false</param>
        /// ///
        /// <param name="password">Password to be tested on file (Leave out, to test without Password)</param>
        private static void PasswordTest(string testFile, bool canOpen, bool canEdit, string password)
        {
            //Testing Opening
            Assert.That(() =>
            {
                using (new PdfReader(testFile, Encoding.Default.GetBytes(password)))
                { }
            },
                canOpen ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());

            //Testing Editing
            Assert.That(() =>
            {
                using (var pdfReader = new PdfReader(testFile, Encoding.Default.GetBytes(password)))
                {
                    var file = testFile + "PasswordTestCopy.pdf";
                    using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        using (new PdfStamper(pdfReader, fileStream))
                        { }
                    }
                    File.Delete(file);
                }
            }, canEdit ? !Throws.Exception.TypeOf<BadPasswordException>() : Throws.Exception.TypeOf<BadPasswordException>());
        }

        /// <summary>
        ///     Checks for the correct PDF version and permission value, according to the pdf security settings.
        /// </summary>
        /// <param name="job">Job with PDF Testfile, current Profile and OwnerPassword</param>
        public static void DoSecurityTest(Job job, bool IsIText)
        {
            foreach (var file in job.OutputFiles)
                DoSecurityTest(file, job.Profile, job.Passwords.PdfOwnerPassword, IsIText);
        }

        /// <summary>
        ///     Checks for the correct PDF version and permission value, according to the pdf security settings.
        /// </summary>
        /// <param name="file">PDF Testfile</param>
        /// <param name="profile">Profile used for processing</param>
        /// <param name="ownerPassword">Owner Password to open secured file (set any value except NULL if encryption was disabled)</param>
        public static void DoSecurityTest(string file, ConversionProfile profile, string ownerPassword, bool isIText)
        {
            if (!profile.PdfSettings.Security.Enabled)
            {
                Assert.That(() =>
                {
                    using (var reader = new PdfReader(file))
                    {
                        Assert.AreEqual(PdfWriter.VERSION_1_4, reader.PdfVersion, "Pdf Version is not set to 1.4.");
                        Assert.AreEqual(-1, reader.GetCryptoMode(), "Encryption mode is not -1 (disabled)");
                        Assert.AreEqual(0, reader.Permissions, "Wrong permission value");
                    }
                }, !Throws.Exception.TypeOf<BadPasswordException>());
                return;
            }

            using (var pdfReader = new PdfReader(file, Encoding.Default.GetBytes(ownerPassword)))
            {
                CheckEncryptionLevel(profile, isIText, pdfReader);
                CheckPermissions(profile, pdfReader);
            }
        }

        private static void CheckPermissions(ConversionProfile profile, PdfReader pdfReader)
        {
            long permissionCode = pdfReader.Permissions;
            Assert.AreEqual(-3904, permissionCode & -3904, "Permission-Bit 7-8 and 13-32 are not true! (PDF-Reference)");
            Assert.AreEqual(-4, permissionCode | -4, "Permission-Bit 1-2 are not false! (PDF-Reference)");

            if (profile.PdfSettings.Security.AllowToCopyContent)
                Assert.AreEqual(PdfWriter.ALLOW_COPY, permissionCode & PdfWriter.ALLOW_COPY,
                    "Requested Allow-Copy is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_COPY,
                    "Unrequested Allow-Copy is set");

            if (profile.PdfSettings.Security.AllowToEditComments)
                Assert.AreEqual(PdfWriter.ALLOW_MODIFY_ANNOTATIONS, permissionCode & PdfWriter.ALLOW_MODIFY_ANNOTATIONS,
                    "Requested Allow-Modify-Annotations is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_MODIFY_ANNOTATIONS,
                    "Unrequested Allow-Modify-Annotations is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");

            if (profile.PdfSettings.Security.AllowToEditTheDocument)
                Assert.AreEqual(PdfWriter.ALLOW_MODIFY_CONTENTS, permissionCode & PdfWriter.ALLOW_MODIFY_CONTENTS,
                    "Requested Allow-Modify-Content is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_MODIFY_CONTENTS,
                    "Unrequested Allow-Modify-Content is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");

            //Printing
            Assert.IsFalse(!profile.PdfSettings.Security.AllowPrinting && profile.PdfSettings.Security.RestrictPrintingToLowQuality, "Restrict to degraded printing is set without allowed printing");

            if (profile.PdfSettings.Security.AllowPrinting && (!profile.PdfSettings.Security.RestrictPrintingToLowQuality || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit)))
                Assert.AreEqual(PdfWriter.ALLOW_PRINTING, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Requested Allow-Printing is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (!profile.PdfSettings.Security.AllowPrinting && (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(2048, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Requested Allow-Printing is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (!profile.PdfSettings.Security.AllowPrinting && !profile.PdfSettings.Security.RestrictPrintingToLowQuality)
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "Unrequested Allow-Printing is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else if (profile.PdfSettings.Security.AllowPrinting && profile.PdfSettings.Security.RestrictPrintingToLowQuality)
                Assert.AreEqual(PdfWriter.ALLOW_DEGRADED_PRINTING, permissionCode & PdfWriter.ALLOW_PRINTING,
                    "No Restriction to degraded printing (" + profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption
            if (profile.PdfSettings.Security.AllowToEditAssembly || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_ASSEMBLY, permissionCode & PdfWriter.ALLOW_ASSEMBLY,
                    "Requested Allow-Assembly is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_ASSEMBLY,
                    "Unrequested Allow-Assembly is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption
            if (profile.PdfSettings.Security.AllowToFillForms || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_FILL_IN, permissionCode & PdfWriter.ALLOW_FILL_IN,
                    "Requested Allow-Fill-In is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_FILL_IN,
                    "Unrequested Allow-Fill-In is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");

            //Extended permission set automatically for 40BitEncryption
            if (profile.PdfSettings.Security.AllowScreenReader || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit))
                Assert.AreEqual(PdfWriter.ALLOW_SCREENREADERS, permissionCode & PdfWriter.ALLOW_SCREENREADERS,
                    "Requested Allow-ScreenReaders is not set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
            else
                Assert.AreEqual(0, permissionCode & PdfWriter.ALLOW_SCREENREADERS,
                    "Unrequested Allow-ScreenReaders is set (" + profile.PdfSettings.Security.EncryptionLevel + ")");
        }

        private static void CheckEncryptionLevel(ConversionProfile profile, bool IsIText, PdfReader pdfReader)
        {
            switch (profile.PdfSettings.Security.EncryptionLevel)
            {
                case EncryptionLevel.Rc40Bit:
                    Assert.AreEqual(0, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for 40Bit");
                    break;

                case EncryptionLevel.Rc128Bit:
                    Assert.AreEqual(1, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for 128Bit");
                    break;

                case EncryptionLevel.Aes128Bit:
                    Assert.AreEqual(2, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for 128BitAES");
                    break;

                case EncryptionLevel.Aes256Bit:
                    if (IsIText)
                    {
                        Assert.AreEqual(2, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for 256AES => 128BitAES");
                        break;
                    }
                    Assert.AreEqual(3, pdfReader.GetCryptoMode(), "Wrong Encrypt-Mode for 256AES => 128BitAES");
                    break;
            }
        }
    }
}
