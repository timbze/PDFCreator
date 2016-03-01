using System;
using NUnit.Framework;
using pdfforge.PDFCreator.Core;
using pdfforge.PDFCreator.Core.Actions;
using pdfforge.PDFCreator.Core.Settings;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest
{
    [TestFixture]
    class ProfileCheckerTest
    {
        private ConversionProfile _profile;
        private ActionResult _result;

        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
        }

        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        private void SetValidAutoSaveSettings()
        {
            _profile.AutoSave.Enabled = true;
            _profile.AutoSave.TargetDirectory = "random autosave directory";
            _profile.FileNameTemplate = "random autosave filename";
        }

        #region Default profile
        [Test]
        public void DefaultProfile_valid()
        {
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Default profile has errors:" + Environment.NewLine + _result);
        }

        [Test]
        public void DefaultProfile_Autosave_valid()
        {
            SetValidAutoSaveSettings();
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Default profile with valid autosave has errors:" + Environment.NewLine+ _result);
        }

        [Test]
        public void DefaultProfile_Autosave_NoFilename()
        {
            _profile.AutoSave.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.FileNameTemplate = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(21101, _result, "ProfileCheck did not detect missing filename template for autosave.");
            _result.Remove(21101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void DefaultProfile_Autosave_NoDirectory()
        {
            _profile.AutoSave.Enabled = true;
            _profile.AutoSave.TargetDirectory = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(21100, _result, "ProfileCheck did not detect missing directory for autosave.");
            _result.Remove(21100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void DefaultProfile_MultipleErrors()
        {
            _profile.AutoSave.Enabled = true;
            _profile.FileNameTemplate = "";
            _profile.AutoSave.TargetDirectory = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(21101, _result, "ProfileCheck did not detect missing filename template for autosave.");
            _result.Remove(21101);
            Assert.Contains(21100, _result, "ProfileCheck did not detect missing directory for autosave.");
            _result.Remove(21100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion

        #region Save Settings

        [Test]
        public void EmptyFolderForSaveDialog()
        {
            _profile.SaveDialog.SetDirectory = true;
            _profile.SaveDialog.Folder = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(28100, _result, "ProfileCheck did not detect empty folder for save dialog.");
            _result.Remove(28100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void EmptyFolderForSaveDialog_EnabledAutosave_IsValid()
        {
            _profile.SaveDialog.SetDirectory = true;
            _profile.SaveDialog.Folder = "";
            _profile.AutoSave.Enabled = true;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsFalse(_result.Contains(28100), "ProfileCheck should ignore empty folder for save dialog if Autosave is enabled.");
        }

        #endregion

        #region CoverPage Settings

        [Test]
        public void CoverPageSettings_valid()
        {
            _profile.CoverPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.pdf");
            _profile.CoverPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Valid profile for CoverPage contains errors:" + Environment.NewLine + _result);
        }

        [Test]
        public void CoverPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.CoverPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.Pdf");
            _profile.CoverPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Valid profile for CoverPage contains errors:" + Environment.NewLine + _result);
        }

        [Test]
        public void CoverPageSettings_NoCoverFile()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(22100, _result, "Profile check did not detect missing cover file.");
            _result.Remove(22100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFile()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(22101, _result, "Profile check did not detect, that the cover file does not exist.");
            _result.Remove(22101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void CoverPageSettings_ExistingCoverFileButNoPDF()
        {
            _profile.CoverPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.noPdf");
            _profile.CoverPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(22102, _result, "Profile check did not detect, that the cover file is no PDF.");
            _result.Remove(22102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFileInNetworkPathIsPdf()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFileInNetworkPathIsNoPdf()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(22102, _result, "Profile check did not detect, that the cover file is no PDF.");
            _result.Remove(22102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        #endregion

        #region AttachmentPage Settings

        [Test]
        public void AttachmentPageSettings_valid()
        {
            _profile.AttachmentPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.pdf");
            _profile.AttachmentPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Valid profile for AttachmentPage contains errors:" + Environment.NewLine + _result);
        }

        [Test]
        public void AttachmentPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.AttachmentPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.Pdf");
            _profile.AttachmentPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Valid profile for AttachmentPage contains errors:" + Environment.NewLine + _result);
        }

        [Test]
        public void AttachmentPageSettings_NoAttachmentFile()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(23100, _result, "Profile check did not detect missing attachment file.");
            _result.Remove(23100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFile()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(23101, _result, "Profile check did not detect, that the attachment file does not exist.");
            _result.Remove(23101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void AttachmentPageSettings_ExistingAttachmentFileButNoPDF()
        {
            _profile.AttachmentPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.noPdf");
            _profile.AttachmentPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(23102, _result, "Profile check did not detect, that the attachment file is no PDF.");
            _result.Remove(23102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }
        
        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFileInNetworkPathIsPdf()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFileInNetworkPathIsNoPdf()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(23102, _result, "Profile check did not detect, that the attachment file is no PDF.");
            _result.Remove(23102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        #endregion

        #region Security Settings
        [Test]
        public void SecuritySettings_NoAutosave_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SecuritySettings_Autosave_RequireUserPwDisabeled_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = false;
            _profile.PdfSettings.Security.UserPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwDisabled_NoOwnerPw()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = false;
            _profile.PdfSettings.Security.UserPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(25100, _result, "ProfileCheck did not detect, missing owner password for autosave.");
            _result.Remove(25100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnabled_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "1234";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnabled_NoUserPw()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(25101, _result, "ProfileCheck did not detect, missing user password for autosave.");
            _result.Remove(25101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnable_MultipleErrors()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(25100, _result, "ProfileCheck did not detect, missing owner password for autosave.");
            _result.Remove(25100);
            Assert.Contains(25101, _result, "ProfileCheck did not detect, missing user password for autosave.");
            _result.Remove(25101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion
        
        #region Signing Settings
        [Test]
        public void SigningSettings_NoAutosave_NoSecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SigningSettings_NoAutoSave_NoCertFile()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12100, _result, "ProfileCheck did not detect missing certification file.");
            _result.Remove(12100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SigningSettings_NoAutoSave_NotExistingCertFile()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "does_not_exist_3912839021830.psx";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12101, _result, "ProfileCheck did not detect, that the certification file does not exist.");
            _result.Remove(12101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SigningSettings_NoAutoSave_NotExistingCertificateFileInNetworkPath()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.psx";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SigningSettings_AutoSave_NoSecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "1234";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SigningSettings_AutoSave_SecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "1234";
            _profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _profile.PdfSettings.Signature.TimeServerLoginName = "SecuredTimeServerLoginName";
            _profile.PdfSettings.Signature.TimeServerPassword = "SecuredTimeServerPassword";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SigningSettings_AutoSave_NoPassword()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = false;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12102, _result, "ProfileCheck did not detect missing certification password for autosave.");
            _result.Remove(12102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _profile.PdfSettings.Signature.TimeServerLoginName = "SecuredTimeServerLoginName";
            _profile.PdfSettings.Signature.TimeServerPassword = "SecuredTimeServerPassword";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_MissingTimeServerLoginName()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "SecuredTimeServerPassword";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12103, _result, "ProfileCheck did not detect missing login name for secured time server.");
            _result.Remove(12103);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_MissingTimeServerPassword()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "certification_file_dummy.whatever");
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _profile.PdfSettings.Signature.TimeServerLoginName = "SecuredTimeServerLoginName";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12104, _result, "ProfileCheck did not detect missing password for secured time server.");
            _result.Remove(12104);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
   
        [Test]
        public void SigningSettings_AutoSave_SecuredTimeServer_MultipleErrors()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "does_not_exist_3912839021830.psx";
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _profile.PdfSettings.Signature.TimeServerIsSecured = true;
            _profile.PdfSettings.Signature.TimeServerLoginName = "";
            _profile.PdfSettings.Signature.TimeServerPassword = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(12101, _result, "ProfileCheck did not detect, that the certification file does not exist.");
            _result.Remove(12101);
            Assert.Contains(12102, _result, "ProfileCheck did not detect missing certification password for autosave.");
            _result.Remove(12102);
            Assert.Contains(12103, _result, "ProfileCheck did not detect missing login name for secured time server.");
            _result.Remove(12103);
            Assert.Contains(12104, _result, "ProfileCheck did not detect missing password for secured time server.");
            _result.Remove(12104);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion

        #region FTP Settings
        [Test]
        public void FTPSettings_NoAutosave_valid()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _profile.Ftp.Server = "random ftp server";
            _profile.Ftp.UserName = "random ftp username";
            _profile.AutoSave.Enabled = false;
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void FTPSettings_NoAutosave_NoServer()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _profile.Ftp.Server = "";
            _profile.Ftp.UserName = "random ftp username";
            _profile.AutoSave.Enabled = false;
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(18100, _result, "ProfileCheck did not detect missing FTP server.");
            _result.Remove(18100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void FTPSettings_NoAutosave_NoUsername()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _profile.Ftp.Server = "random ftp server";
            _profile.Ftp.UserName = "";
            _profile.AutoSave.Enabled = false;
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(18101, _result, "ProfileCheck did not detect missing FTP username.");
            _result.Remove(18101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void FTPSettings_NoAutosave_MultipleErrors()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "";
            _profile.Ftp.Server = "";
            _profile.Ftp.UserName = "";
            _profile.AutoSave.Enabled = false;
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(18100, _result, "ProfileCheck did not detect missing FTP server.");
            _result.Remove(18100);
            Assert.Contains(18101, _result, "ProfileCheck did not detect missing FTP username.");
            _result.Remove(18101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void FTPSettings_Autosave_valid()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _profile.Ftp.Server = "random ftp server";
            _profile.Ftp.UserName = "random ftp username";
            SetValidAutoSaveSettings();
            _profile.Ftp.Password = "1234";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void FTPSettings_Autosave_NoPassword()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _profile.Ftp.Server = "random ftp server";
            _profile.Ftp.UserName = "random ftp username";
            SetValidAutoSaveSettings();
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(18109, _result, "ProfileCheck did not detect missing FTP password for autosave.");
            _result.Remove(18109);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result); 
        }

        [Test]
        public void FTPSettings_Autosave_MultipleErrors()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "";
            _profile.Ftp.Server = "";
            _profile.Ftp.UserName = "";
            SetValidAutoSaveSettings();
            _profile.Ftp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(18100, _result, "ProfileCheck did not detect missing FTP server.");
            _result.Remove(18100);
            Assert.Contains(18101, _result, "ProfileCheck did not detect missing FTP username.");
            _result.Remove(18101);
            Assert.Contains(18109, _result, "ProfileCheck did not detect missing FTP password for autosave.");
            _result.Remove(18109);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion

        #region Script Settings
        [Test]
        public void ScriptSettings_valid()
        {
            _profile.Scripting.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "ScriptfielDummy.exe");
            _profile.Scripting.ScriptFile = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void ScriptSettings_NoScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(14100, _result, "ProfileCheck did not detect missing script file.");
            _result.Remove(14100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void ScriptSettings_NotExistingScriptFile()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = "Doesnotexist.exe";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(14101, _result, "ProfileCheck did not detect, that the script file does not exist.");
            _result.Remove(14101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void ScriptSettings_NotExistingSkriptFileInNetworkPath()
        {
            _profile.Scripting.Enabled = true;
            _profile.Scripting.ScriptFile = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.exe";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void ScriptSettings_ScriptFile_Result_must_be_valid()
        {
            _profile.Scripting.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "ScriptfielDummy.exe");
            _profile.Scripting.ScriptFile = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Valid profile for Scripting (without Autosave) contains errors:" + Environment.NewLine + _result);
        }
        #endregion

        #region SMTP Settings
        [Test]
        public void SmtpSettings_NoAutoSave_valid()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoAdress()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15100, _result, "ProfileCheck did not detect missing SMTP adress.");
            _result.Remove(15100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoUserName()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15104, _result, "ProfileCheck did not detect missing SMTP username.");
            _result.Remove(15104);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoHost()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15102, _result, "ProfileCheck did not detect missing SMTP host.");
            _result.Remove(15102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_InvalidPort()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = -1;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15103, _result, "ProfileCheck did not detect invalid SMTP port.");
            _result.Remove(15103);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoRecipients()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15101, _result, "ProfileCheck did not detect missing SMTP recipients.");
            _result.Remove(15101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_MultipleErrors()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;
            
            _profile.EmailSmtp.Address = "";
            _profile.EmailSmtp.UserName = "";
            _profile.EmailSmtp.Server = "";
            _profile.EmailSmtp.Port = -1;
            _profile.EmailSmtp.Recipients = "";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15100, _result, "ProfileCheck did not detect missing SMTP adress.");
            _result.Remove(15100);
            Assert.Contains(15104, _result, "ProfileCheck did not detect missing SMTP username.");
            _result.Remove(15104);
            Assert.Contains(15102, _result, "ProfileCheck did not detect missing SMTP host.");
            _result.Remove(15102);
            Assert.Contains(15103, _result, "ProfileCheck did not detect invalid SMTP port.");
            _result.Remove(15103);
            Assert.Contains(15101, _result, "ProfileCheck did not detect missing SMTP recipients.");
            _result.Remove(15101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);   
        }

        [Test]
        public void SmtpSettings_AutoSave_valid()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            _profile.EmailSmtp.Password = "1234";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_AutoSave_NoPassword()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtp.UserName = "randomUsername";
            _profile.EmailSmtp.Server = "randomHost";
            _profile.EmailSmtp.Port = 25;
            _profile.EmailSmtp.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15110, _result, "ProfileCheck did not detect missing SMTP password for autosaving.");
            _result.Remove(15110);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void SmtpSettings_AutoSave_MultipleErrors()
        {
            _profile.EmailSmtp.Enabled = true;
            _profile.EmailSmtp.Ssl = false;
            _profile.EmailSmtp.Subject = "";
            _profile.EmailSmtp.Content = "";
            _profile.EmailSmtp.AddSignature = false;

            _profile.EmailSmtp.Address = "";
            _profile.EmailSmtp.UserName = "";
            _profile.EmailSmtp.Server = "";
            _profile.EmailSmtp.Port = -1;
            _profile.EmailSmtp.Recipients = "";
            SetValidAutoSaveSettings();
            _profile.EmailSmtp.Password = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(15100, _result, "ProfileCheck did not detect missing SMTP adress.");
            _result.Remove(15100);
            Assert.Contains(15104, _result, "ProfileCheck did not detect missing SMTP username.");
            _result.Remove(15104);
            Assert.Contains(15102, _result, "ProfileCheck did not detect missing SMTP host.");
            _result.Remove(15102);
            Assert.Contains(15103, _result, "ProfileCheck did not detect invalid SMTP port.");
            _result.Remove(15103);
            Assert.Contains(15101, _result, "ProfileCheck did not detect missing SMTP recipients.");
            _result.Remove(15101);
            Assert.Contains(15110, _result, "ProfileCheck did not detect missing SMTP password for autosaving.");
            _result.Remove(15110);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion

        #region BackgroundPage Settings
        [Test]
        public void BackgroundPageSettings_valid()
        {
            _profile.BackgroundPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.pdf");
            _profile.BackgroundPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.BackgroundPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.Pdf");
            _profile.BackgroundPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_NoBackgroundFile()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(17100, _result, "Profile check did not detect missing Background file.");
            _result.Remove(17100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFile()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(17101, _result, "Profile check did not detect, that the Background file does not exist.");
            _result.Remove(17101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_ExistingBackgroundFileButNoPDF()
        {
            _profile.BackgroundPage.Enabled = true;
            var testFile = TempFileHelper.CreateTempFile("ProfileCheckerTest", "testfile.noPdf");
            _profile.BackgroundPage.File = testFile;
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(17102, _result, "Profile check did not detect, that the Background file is no PDF.");
            _result.Remove(17102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFileInNetworkPathIsPdf()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFileInNetworkPathIsNoPdf()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(17102, _result, "Profile check did not detect, that the Background file is no PDF.");
            _result.Remove(17102);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        #endregion

        #region Stamping Settings
        [Test]
        public void StampingSettings_valid()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "Stamp Text";
            _profile.Stamping.FontName = "Arial";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void StampingSettings_NoStamptext()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "";
            _profile.Stamping.FontName = "Arial";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(24100, _result, "Profile check did not detect missing stamp text.");
            _result.Remove(24100);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }

        [Test]
        public void StampingSettings_NoFontname()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "Stamp Text";
            _profile.Stamping.FontName = "";
            _result = ProfileChecker.ProfileCheck(_profile);
            Assert.Contains(24101, _result, "Profile check did not detect missing font name.");
            _result.Remove(24101);
            Assert.IsTrue(_result, "Unexpected errorcodes:" + Environment.NewLine + _result);
        }
        #endregion
    } 
}
