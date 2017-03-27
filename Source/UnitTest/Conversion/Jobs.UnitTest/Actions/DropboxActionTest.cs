using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class DropboxActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
            _profile.DropboxSettings.AccountId = accountId;
            _dropboxService = Substitute.For<IDropboxService>();
            _dropboxSharedLinks = Substitute.For<IDropboxSharedLinksProvider>();

            _accounts = new Accounts();
            _accounts.DropboxAccounts.Add(new DropboxAccount { AccountId = accountId });
            _accounts.DropboxAccounts.Add(new DropboxAccount { AccountId = accountId, AccessToken = "aaa" });
            action = new DropboxAction(_dropboxService, _dropboxSharedLinks);
        }

        private Job _job;
        private DropboxAction action;
        private IDropboxService _dropboxService;
        private IDropboxSharedLinksProvider _dropboxSharedLinks;
        private readonly string accountId = "myaccountid";
        private readonly string acccessToken = "acccessToken";
        private ConversionProfile _profile;
        private Accounts _accounts;

        private Job BuildJob()
        {
            return new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
        }

        [Test]
        public void Can_Init_Job()
        {
            var profile = new ConversionProfile();
            _job = new Job(new JobInfo(), profile, new JobTranslations(), _accounts);
            Assert.IsTrue(action.Init(_job));
        }


        #region Testing isCheck method
        [Test]
        public void Check_DropBox_Settings_AccessToken_IsNullOrEmpty()
        {
            _profile.DropboxSettings.Enabled = true;

            var accounts = new Accounts();
            accounts.DropboxAccounts.Add(new DropboxAccount
            {
                AccountId = accountId,
                AccessToken = string.Empty
            });

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), accounts);

            var result = action.Check(_profile, _accounts);
            Assert.AreEqual(ErrorCode.Dropbox_AccessTokenNotSpecified, result.FirstOrDefault());
        }


        [Test]
        public void Check_DropBox_Settings_DropboxAptionNotEnabled_ReturnFalse()
        {
            _profile.DropboxSettings.Enabled = false;

            var result = action.Check(_profile, new Accounts());
            Assert.AreEqual(result, new ActionResult());
        }

        [Test]
        public void Check_DropBox_Settings_EverythingOK()
        {
            var accounts = new Accounts();
            accounts.DropboxAccounts.Add(new DropboxAccount
            {
                AccountId = accountId,
                AccessToken = acccessToken
            });

            _profile.DropboxSettings.Enabled = true;

            var result = action.Check(_profile, accounts);
            Assert.AreEqual(result, new ActionResult());
        }

        [Test]
        public void Check_InvalidDropboxSharedFolderName_ReturnsErrorCodeInvalidFoderName()
        {
            _profile.DropboxSettings.Enabled = true;

            var accounts = new Accounts();
            accounts.DropboxAccounts.Add(new DropboxAccount
            {
                AccountId = accountId,
                AccessToken = acccessToken
            });

            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.SharedFolder = "!pdfforge.";
            var result = action.Check(_profile, accounts);
            Assert.AreEqual(ErrorCode.Dropbox_InvalidFolderName, result.FirstOrDefault());
        }


        [Test]
        public void Check_EverythingOkWithTokenInsideFolderName_ReturnsTrue()
        {
            _profile.DropboxSettings.Enabled = true;

            var accounts = new Accounts();
            accounts.DropboxAccounts.Add(new DropboxAccount
            {
                AccountId = accountId,
                AccessToken = acccessToken
            });

            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.SharedFolder = "<DateTime> PDfForge";
            var result = action.Check(_profile, accounts);
            Assert.AreEqual(result, new ActionResult());
        }
        #endregion
        [Test]
        public void Check_Is_DropBoxSetting_Enabled()
        {
            var profile = new ConversionProfile();
            profile.DropboxSettings.Enabled = true;

            _job = new Job(new JobInfo(), profile, new JobTranslations(), _accounts);
            Assert.IsTrue(action.IsEnabled(profile));
        }




        [Test]
        public void DropBoxSettingsNoAccountDefined_ReturnsErrorDropbox_AccountNotSpecified()
        {
            _profile.DropboxSettings.Enabled = true;

            _job = BuildJob();

            var result = action.Check(_profile, new Accounts());
            Assert.AreEqual(ErrorCode.Dropbox_AccountNotSpecified, result.FirstOrDefault());
        }




        [Test]
        public void UploadFile_ReturnsActionResult()
        {
            _profile.DropboxSettings.Enabled = true;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            action.ProcessJob(_job);

            _dropboxService.Received()
                .UploadFiles("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, _job.JobTempOutputFolder);
        }

        [Test]
        public void DropBoxSettingsInvalidFolderName_ReturnsErrorDropbox_InvalidFolderName()
        {
            _profile.DropboxSettings.Enabled = true;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);

            _profile.DropboxSettings.SharedFolder += ": ?";
            var result = action.Check(_profile, _job.Accounts);
            Assert.AreEqual(ErrorCode.Dropbox_InvalidFolderName, result.FirstOrDefault());
        }


        [Test]
        public void UploadFileWithSharedLink_CheckIsTokenReplacerFilledWithLink()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;


            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _dropboxService.UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, false, _job.OutputFilenameTemplate).Returns(
                new DropboxFileMetaData
                {
                    FilePath = "/File1.pdf", SharedUrl = "htttps://dropbox.com/File1.pdf"
                });

            action.ProcessJob(_job);
            var tokenNames = _job.TokenReplacer.GetTokenNames(false);
            Assert.Contains("DROPBOXHTMLLINKS", tokenNames);
            Assert.Contains("DROPBOXFULLLINKS", tokenNames);
        }

        [Test]
        public void UploadFileWithSharedLink_ReturnsEmptyActionResult()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            action.ProcessJob(_job);

            _dropboxService.Received().UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, _job.OutputFilenameTemplate);
        }

        [Test]
        public void UploadFileWithSharedLink_WhenFailsToUpload_ReturnsErrorCode()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxService.UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, string.Empty).Returns(new DropboxFileMetaData());
            var result = action.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Dropbox_Upload_And_Share_Error, result.FirstOrDefault());
        }

        [Test]
        public void UploadFile_WhenFailsToUpload_ReturnsErrorCode()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = false;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();

            _dropboxService.UploadFiles("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames, string.Empty).Returns(false);
            var result = action.ProcessJob(_job);
            Assert.AreEqual(ErrorCode.Dropbox_Upload_Error, result.FirstOrDefault());
        }
    }
}
