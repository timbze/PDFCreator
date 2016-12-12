using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
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

            _accounts = new Accounts();
            _accounts.DropboxAccounts.Add(new DropboxAccount {AccountId = accountId});
            _accounts.DropboxAccounts.Add(new DropboxAccount {AccountId = accountId, AccessToken = "aaa"});
        }

        private Job _job;
        private IDropboxService _dropboxService;
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
            var action = new DropboxAction(_dropboxService);
            Assert.IsTrue(action.Init(_job));
        }


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

            var action = new DropboxAction(_dropboxService);
            var result = action.Check(_profile, _accounts);
            Assert.AreEqual(ErrorCode.Dropbox_AccessTokenNotSpecified, result.FirstOrDefault());
        }

        [Test]
        public void Check_DropBox_Settings_DropboxAptionNotEnabled_ReturnFalse()
        {
            _profile.DropboxSettings.Enabled = false;

            var action = new DropboxAction(_dropboxService);
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

            var action = new DropboxAction(_dropboxService);
            var result = action.Check(_profile, accounts);
            Assert.AreEqual(result, new ActionResult());
        }


        [Test]
        public void Check_Is_DropBoxSetting_Enabled()
        {
            var profile = new ConversionProfile();
            profile.DropboxSettings.Enabled = true;

            _job = new Job(new JobInfo(), profile, new JobTranslations(), _accounts);
            var action = new DropboxAction(_dropboxService);
            Assert.IsTrue(action.IsEnabled(profile));
        }


        [Test]
        public void DropBoxSettingsNoAccountDefined_ReturnsErrorDropbox_AccountNotSpecified()
        {
            _profile.DropboxSettings.Enabled = true;

            _job = BuildJob();

            var action = new DropboxAction(_dropboxService);
            var result = action.Check(_profile, new Accounts());
            Assert.AreEqual(ErrorCode.Dropbox_AccountNotSpecified, result.FirstOrDefault());
        }

        [Test]
        public void UploadFile_ReturnsActionResult()
        {
            _profile.DropboxSettings.Enabled = true;

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] {@"C:\Temp\file1.pdf"}.ToList();
            var action = new DropboxAction(_dropboxService);

            action.ProcessJob(_job);

            _dropboxService.Received()
                .UploadFiles("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames);
        }

        [Test]
        public void UploadFileWithSharedLink_CheckIsTokenReplacerFilledWithLink()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;


            var action = new DropboxAction(_dropboxService);
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _dropboxService.UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, false).Returns(
                new List<DropboxFileMetaData>
                {
                    new DropboxFileMetaData {FilePath = "/File1.pdf", SharedUrl = "htttps://dropbox.com/File1.pdf"},
                    new DropboxFileMetaData {FilePath = "/File2.pdf", SharedUrl = "htttps://dropbox.com/File2.pdf"}
                });

            action.ProcessJob(_job);
            var tokenNames = _job.TokenReplacer.GetTokenNames(false);
            Assert.Contains("DROPBOXHTMLLINKS", tokenNames);
            Assert.Contains("DROPBOXFULLLINKS", tokenNames);
        }

        [Test]
        public void UploadFileWithSharedLink_ReturnsActionResult()
        {
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.OutputFiles = new[] {@"C:\Temp\file1.pdf"}.ToList();
            var action = new DropboxAction(_dropboxService);

            action.ProcessJob(_job);

            _dropboxService.Received().UploadFileWithSharing("", _profile.DropboxSettings.SharedFolder, _job.OutputFiles, _profile.DropboxSettings.EnsureUniqueFilenames);
        }
    }
}
