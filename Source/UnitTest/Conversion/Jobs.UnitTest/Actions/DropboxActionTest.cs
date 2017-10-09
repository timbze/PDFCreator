using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class DropboxActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.AccountId = _accountId;

            _dropboxService = Substitute.For<IDropboxService>();

            _dropboxAccount = new DropboxAccount();
            _dropboxAccount.AccountId = _accountId;
            _dropboxAccount.AccessToken = _accessToken;

            _accounts = new Accounts();
            _accounts.DropboxAccounts.Add(_dropboxAccount);

            _dropboxAction = new DropboxAction(_dropboxService);
        }

        private Job _job;
        private DropboxAction _dropboxAction;
        private IDropboxService _dropboxService;
        private readonly string _accountId = "myaccountid";
        private readonly string _accessToken = "acccessToken";
        private ConversionProfile _profile;
        private DropboxAccount _dropboxAccount;
        private Accounts _accounts;

        #region Testing isCheck method

        [Test]
        public void Check_DropBox_Settings_AccessToken_IsNullOrEmpty()
        {
            _profile.DropboxSettings.Enabled = true;
            _dropboxAccount.AccountId = _accountId;
            _dropboxAccount.AccessToken = string.Empty;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);

            var result = _dropboxAction.Check(_profile, _accounts);
            Assert.AreEqual(ErrorCode.Dropbox_AccessTokenNotSpecified, result.FirstOrDefault());
        }

        [Test]
        public void Check_ValidDropBoxSettings_EverythingOK()
        {
            var result = _dropboxAction.Check(_profile, _accounts);

            Assert.AreEqual(result, new ActionResult());
        }

        [Test]
        public void Check_InvalidDropboxSharedFolderName_ReturnsErrorCodeInvalidFoderName()
        {
            _profile.DropboxSettings.SharedFolder = "!pdfforge.";

            var result = _dropboxAction.Check(_profile, _accounts);

            Assert.AreEqual(ErrorCode.Dropbox_InvalidFolderName, result.FirstOrDefault());
        }

        [Test]
        public void Check_EverythingOkWithTokenInsideFolderName_ReturnsTrue()
        {
            _profile.DropboxSettings.SharedFolder = "<DateTime> PDfForge";

            var result = _dropboxAction.Check(_profile, _accounts);

            Assert.AreEqual(result, new ActionResult());
        }

        #endregion Testing isCheck method

        [Test]
        public void Check_Is_DropBoxSetting_Enabled()
        {
            var profile = new ConversionProfile();
            profile.DropboxSettings.Enabled = true;

            Assert.IsTrue(_dropboxAction.IsEnabled(profile));
        }

        [Test]
        public void DropBoxSettingsNoAccountDefined_ReturnsErrorDropbox_AccountNotSpecified()
        {
            _profile.DropboxSettings.AccountId = "Some unavailable ID";

            var result = _dropboxAction.Check(_profile, _accounts);

            Assert.AreEqual(ErrorCode.Dropbox_AccountNotSpecified, result.FirstOrDefault());
        }

        [Test]
        public void UploadFile_ReturnsActionResult()
        {
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxAction.ProcessJob(_job);

            _dropboxService.Received()
                .UploadFiles(_accessToken, _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, _job.JobTempOutputFolder);
        }

        [Test]
        public void UploadFile_WithBackslash_ConvertsToforwardSlash()
        {
            _profile.DropboxSettings.SharedFolder += "\\test";
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxAction.ProcessJob(_job);

            var sanitizedFolder = _profile.DropboxSettings.SharedFolder.Replace("\\", "/");

            _dropboxService.Received()
                .UploadFiles(_accessToken, sanitizedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, _job.JobTempOutputFolder);
        }

        [Test]
        public void DropBoxSettingsInvalidFolderName_ReturnsErrorDropbox_InvalidFolderName()
        {
            _profile.DropboxSettings.SharedFolder += ": ?";

            var result = _dropboxAction.Check(_profile, _accounts);

            Assert.AreEqual(ErrorCode.Dropbox_InvalidFolderName, result.FirstOrDefault());
        }

        [Test]
        public void UploadFileWithSharedLink_CheckIsTokenReplacerFilledWithLink()
        {
            _profile.DropboxSettings.CreateShareLink = true;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _dropboxService.UploadFileWithSharing(_accessToken, _profile.DropboxSettings.SharedFolder, _job.OutputFiles, false, _job.OutputFilenameTemplate).Returns(
                new DropboxFileMetaData
                {
                    Filename = "/File1.pdf",
                    ShareUrl = "htttps://dropbox.com/File1.pdf"
                });
            _dropboxAction.ProcessJob(_job);
            var tokenNames = _job.TokenReplacer.GetTokenNames(false);
            Assert.Contains("DROPBOXHTMLLINKS", tokenNames);
            Assert.Contains("DROPBOXFULLLINKS", tokenNames);
        }

        [Test]
        public void UploadFileWithSharedLink_ShareLinkIsSetInJob()
        {
            _profile.DropboxSettings.CreateShareLink = true;
            var shareUrl = "htttps://dropbox.com/File1.pdf";
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _dropboxService.UploadFileWithSharing(_accessToken, _profile.DropboxSettings.SharedFolder, _job.OutputFiles, false, _job.OutputFilenameTemplate).Returns(
                new DropboxFileMetaData
                {
                    Filename = "/File1.pdf",
                    ShareUrl = shareUrl
                });

            _dropboxAction.ProcessJob(_job);

            Assert.AreEqual(shareUrl, _job.ShareLinks.DropboxShareUrl);
        }

        [Test]
        public void UploadFileWithSharedLink_ReturnsEmptyActionResult()
        {
            _profile.DropboxSettings.CreateShareLink = true;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxAction.ProcessJob(_job);

            _dropboxService.Received().UploadFileWithSharing(_accessToken, _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, _job.OutputFilenameTemplate);
        }

        [Test]
        public void UploadFileWithSharedLink_WhenFailsToUpload_ReturnsErrorCode()
        {
            _profile.DropboxSettings.CreateShareLink = true;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxService.UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, string.Empty).Returns(new DropboxFileMetaData());
            var result = _dropboxAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Dropbox_Upload_And_Share_Error, result.FirstOrDefault());
        }

        [Test]
        public void UploadFile_WhenFailsToUpload_ReturnsErrorCode()
        {
            _profile.DropboxSettings.CreateShareLink = false;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxService.UploadFiles("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, string.Empty).Returns(false);
            var result = _dropboxAction.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Dropbox_Upload_Error, result.FirstOrDefault());
        }
    }
}
