using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using static pdfforge.PDFCreator.Conversion.Jobs.ErrorCode;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs
{
    [TestFixture]
    internal class ProfileCheckerTest
    {
        private TimeServerAccount _timeServerAccount;
        private ConversionProfile _profile;
        private IFile _file;
        private ProfileChecker _profileChecker;
        private List<ICheckable> _actionCheckList;
        private Accounts _accounts;

        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
            _file = Substitute.For<IFile>();
            _actionCheckList = new List<ICheckable>();
            _actionCheckList.Add(Substitute.For<ICheckable>());
            _actionCheckList.Add(Substitute.For<ICheckable>());
            _actionCheckList.Add(Substitute.For<ICheckable>());

            _accounts = new Accounts();
            _timeServerAccount = new TimeServerAccount();
            _timeServerAccount.AccountId = "TimeServerTestId";
            _profile.PdfSettings.Signature.TimeServerAccountId = _timeServerAccount.AccountId;
            _accounts.TimeServerAccounts.Add(_timeServerAccount);

            for (var i = 0; i < _actionCheckList.Count; i++)
            {
                _actionCheckList[i].Check(_profile, _accounts).Returns(new ActionResult());
            }

            _profileChecker = new ProfileChecker(_file, _actionCheckList);
        }

        private void SetValidAutoSaveSettings()
        {
            _profile.AutoSave.Enabled = true;
            _profile.TargetDirectory = "random autosave directory";
            _profile.FileNameTemplate = "random autosave filename";
        }

        [Test]
        public void AttachmentPageSettings_ExistingAttachmentFileButNoPDF()
        {
            _profile.AttachmentPage.Enabled = true;
            const string testFile = "testfile.noPdf";
            _file.Exists(testFile).Returns(true);
            _profile.AttachmentPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Attachment_NoPdf, result, "Profile check did not detect, that the attachment file is no PDF.");
            result.Remove(Attachment_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_NoAttachmentFile()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Attachment_NoFileSpecified, result, "Profile check did not detect missing attachment file.");
            result.Remove(Attachment_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFile()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = "does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Attachment_FileDoesNotExist, result, "Profile check did not detect, that the attachment file does not exist.");
            result.Remove(Attachment_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFileInNetworkPathIsNoPdf()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Attachment_NoPdf, result, "Profile check did not detect, that the attachment file is no PDF.");
            result.Remove(Attachment_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_NotExistingAttachmentFileInNetworkPathIsPdf()
        {
            _profile.AttachmentPage.Enabled = true;
            _profile.AttachmentPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_valid()
        {
            _profile.AttachmentPage.Enabled = true;
            const string testFile = "testfile.pdf";
            _file.Exists(testFile).Returns(true);
            _profile.AttachmentPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Valid profile for AttachmentPage contains errors:" + Environment.NewLine + result);
        }

        [Test]
        public void AttachmentPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.AttachmentPage.Enabled = true;
            const string testFile = "testfile.Pdf";
            _file.Exists(testFile).Returns(true);
            _profile.AttachmentPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Valid profile for AttachmentPage contains errors:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_ExistingBackgroundFileButNoPDF()
        {
            _profile.BackgroundPage.Enabled = true;
            const string testFile = "testfile.noPdf";
            _file.Exists(testFile).Returns(true);
            _profile.BackgroundPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Background_NoPdf, result, "Profile check did not detect, that the Background file is no PDF.");
            result.Remove(Background_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_NoBackgroundFile()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Background_NoFileSpecified, result, "Profile check did not detect missing Background file.");
            result.Remove(Background_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFile()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = "does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Background_FileDoesNotExist, result, "Profile check did not detect, that the Background file does not exist.");
            result.Remove(Background_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFileInNetworkPathIsNoPdf()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Background_NoPdf, result, "Profile check did not detect, that the Background file is no PDF.");
            result.Remove(Background_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_NotExistingBackgroundFileInNetworkPathIsPdf()
        {
            _profile.BackgroundPage.Enabled = true;
            _profile.BackgroundPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_valid()
        {
            _profile.BackgroundPage.Enabled = true;
            const string testFile = "testfile.Pdf";
            _file.Exists(testFile).Returns(true);
            _profile.BackgroundPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void BackgroundPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.BackgroundPage.Enabled = true;
            const string testFile = "testfile.Pdf";
            _file.Exists(testFile).Returns(true);
            _profile.BackgroundPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_ExistingCoverFileButNoPDF()
        {
            _profile.CoverPage.Enabled = true;
            const string testFile = "testfile.noPdf";
            _file.Exists(testFile).Returns(true);
            _profile.CoverPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Cover_NoPdf, result, "Profile check did not detect, that the cover file is no PDF.");
            result.Remove(Cover_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_NoCoverFile()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Cover_NoFileSpecified, result, "Profile check did not detect missing cover file.");
            result.Remove(Cover_NoFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFile()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = "does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Cover_FileDoesNotExist, result, "Profile check did not detect, that the cover file does not exist.");
            result.Remove(Cover_FileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFileInNetworkPathIsNoPdf()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.nopdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Cover_NoPdf, result, "Profile check did not detect, that the cover file is no PDF.");
            result.Remove(Cover_NoPdf);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_NotExistingCoverFileInNetworkPathIsPdf()
        {
            _profile.CoverPage.Enabled = true;
            _profile.CoverPage.File = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.pdf";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_valid()
        {
            _profile.CoverPage.Enabled = true;
            const string testFile = "testfile.pdf";
            _file.Exists(testFile).Returns(true);
            _profile.CoverPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Valid profile for CoverPage contains errors:" + Environment.NewLine + result);
        }

        [Test]
        public void CoverPageSettings_valid_forCamelCasePdfExtension()
        {
            _profile.CoverPage.Enabled = true;
            const string testFile = "testfile.Pdf";
            _file.Exists(testFile).Returns(true);
            _profile.CoverPage.File = testFile;
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Valid profile for CoverPage contains errors:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_Autosave_NoDirectory()
        {
            _profile.AutoSave.Enabled = true;
            _profile.TargetDirectory = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoTargetDirectory, result, "ProfileCheck did not detect missing directory for autosave.");
            result.Remove(AutoSave_NoTargetDirectory);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_Autosave_NoFilename()
        {
            _profile.AutoSave.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.FileNameTemplate = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoFilenameTemplate, result, "ProfileCheck did not detect missing filename template for autosave.");
            result.Remove(AutoSave_NoFilenameTemplate);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_Autosave_valid()
        {
            SetValidAutoSaveSettings();
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Default profile with valid autosave has errors:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_MultipleErrors()
        {
            _profile.AutoSave.Enabled = true;
            _profile.FileNameTemplate = "";
            _profile.TargetDirectory = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoFilenameTemplate, result, "ProfileCheck did not detect missing filename template for autosave.");
            result.Remove(AutoSave_NoFilenameTemplate);
            Assert.Contains(AutoSave_NoTargetDirectory, result, "ProfileCheck did not detect missing directory for autosave.");
            result.Remove(AutoSave_NoTargetDirectory);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void DefaultProfile_valid()
        {
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Default profile has errors:" + Environment.NewLine + result);
        }

        [Test]
        public void ActionChecks_CallsAllResolvedActionChecks()
        {
            var expectedCodes = new Dictionary<ICheckable, ErrorCode>();
            var errorCodes = Enum.GetValues(typeof(ErrorCode)).Cast<ErrorCode>().ToList();

            foreach (var action in _actionCheckList)
            {
                var expectedCode = errorCodes.Skip(_actionCheckList.IndexOf(action)).First();
                action.Check(_profile, _accounts).Returns(new ActionResult(expectedCode));
                expectedCodes[action] = expectedCode;
            }

            var result = _profileChecker.ProfileCheck(_profile, _accounts);

            foreach (var action in _actionCheckList)
            {
                action.Received().Check(_profile, _accounts);
                Assert.IsTrue(result.Contains(expectedCodes[action]));
            }
        }

        [Test]
        public void SecuritySettings_Autosave_RequireUserPwDisabeled_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = false;
            _profile.PdfSettings.Security.UserPassword = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwDisabled_NoOwnerPw()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = false;
            _profile.PdfSettings.Security.UserPassword = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoOwnerPassword, result, "ProfileCheck did not detect, missing owner password for autosave.");
            result.Remove(AutoSave_NoOwnerPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnable_MultipleErrors()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoOwnerPassword, result, "ProfileCheck did not detect, missing owner password for autosave.");
            result.Remove(AutoSave_NoOwnerPassword);
            Assert.Contains(AutoSave_NoUserPassword, result, "ProfileCheck did not detect, missing user password for autosave.");
            result.Remove(AutoSave_NoUserPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnabled_NoUserPw()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(AutoSave_NoUserPassword, result, "ProfileCheck did not detect, missing user password for autosave.");
            result.Remove(AutoSave_NoUserPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SecuritySettings_Autosave_UserPwEnabled_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Security.OwnerPassword = "1234";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "1234";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SecuritySettings_NoAutosave_valid()
        {
            _profile.PdfSettings.Security.Enabled = true;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Security.OwnerPassword = "";
            _profile.PdfSettings.Security.RequireUserPassword = true;
            _profile.PdfSettings.Security.UserPassword = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_AutoSave_NoPassword()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_AutoSaveWithoutCertificatePassword, result, "ProfileCheck did not detect missing certification password for autosave.");
            result.Remove(ProfileCheck_AutoSaveWithoutCertificatePassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_AutoSave_NoSecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "1234";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_AutoSave_SecuredTimeServer_MultipleErrors()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "does_not_exist_3912839021830.psx";
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_CertificateFileDoesNotExist, result, "ProfileCheck did not detect, that the certification file does not exist.");
            result.Remove(ProfileCheck_CertificateFileDoesNotExist);
            Assert.Contains(ProfileCheck_AutoSaveWithoutCertificatePassword, result, "ProfileCheck did not detect missing certification password for autosave.");
            result.Remove(ProfileCheck_AutoSaveWithoutCertificatePassword);
            Assert.Contains(ProfileCheck_SecureTimeServerWithoutUsername, result, "ProfileCheck did not detect missing login name for secured time server.");
            result.Remove(ProfileCheck_SecureTimeServerWithoutUsername);
            Assert.Contains(ProfileCheck_SecureTimeServerWithoutPassword, result, "ProfileCheck did not detect missing password for secured time server.");
            result.Remove(ProfileCheck_SecureTimeServerWithoutPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_AutoSave_SecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            SetValidAutoSaveSettings();
            _profile.PdfSettings.Signature.SignaturePassword = "1234";
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "SecuredTimeServerLoginName";
            _timeServerAccount.Password = "SecuredTimeServerPassword";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutoSave_NoCertFile()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_NoCertificationFileSpecified, result, "ProfileCheck did not detect missing certification file.");
            result.Remove(ProfileCheck_NoCertificationFileSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutosave_NoSecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutoSave_NotExistingCertFile()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = "does_not_exist_3912839021830.psx";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_CertificateFileDoesNotExist, result, "ProfileCheck did not detect, that the certification file does not exist.");
            result.Remove(ProfileCheck_CertificateFileDoesNotExist);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutoSave_NotExistingCertificateFileInNetworkPath()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            _profile.PdfSettings.Signature.CertificateFile = @"\\notexistingnetworkpath_3920ß392932013912\does_not_exist_3912839021830.psx";
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = false;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_MissingTimeServerLoginName()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "";
            _timeServerAccount.Password = "SecuredTimeServerPassword";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_SecureTimeServerWithoutUsername, result, "ProfileCheck did not detect missing login name for secured time server.");
            result.Remove(ProfileCheck_SecureTimeServerWithoutUsername);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_MissingTimeServerPassword()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "SecuredTimeServerLoginName";
            _timeServerAccount.Password = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(ProfileCheck_SecureTimeServerWithoutPassword, result, "ProfileCheck did not detect missing password for secured time server.");
            result.Remove(ProfileCheck_SecureTimeServerWithoutPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SigningSettings_NoAutosave_SecuredTimeServer_valid()
        {
            _profile.PdfSettings.Signature.Enabled = true;
            const string testFile = "certification_file_dummy.whatever";
            _file.Exists(testFile).Returns(true);
            _profile.PdfSettings.Signature.CertificateFile = testFile;
            _profile.AutoSave.Enabled = false;
            _profile.PdfSettings.Signature.SignaturePassword = "";
            _timeServerAccount.IsSecured = true;
            _timeServerAccount.UserName = "SecuredTimeServerLoginName";
            _timeServerAccount.Password = "SecuredTimeServerPassword";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void StampingSettings_NoFontname()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "Stamp Text";
            _profile.Stamping.FontName = "";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Stamp_NoFont, result, "Profile check did not detect missing font name.");
            result.Remove(Stamp_NoFont);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void StampingSettings_NoStamptext()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "";
            _profile.Stamping.FontName = "Arial";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.Contains(Stamp_NoText, result, "Profile check did not detect missing stamp text.");
            result.Remove(Stamp_NoText);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void StampingSettings_valid()
        {
            _profile.Stamping.Enabled = true;
            _profile.Stamping.StampText = "Stamp Text";
            _profile.Stamping.FontName = "Arial";
            var result = _profileChecker.ProfileCheck(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
