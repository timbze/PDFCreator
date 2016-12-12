using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Dropbox.Api;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Dropbox;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class DropboxServiceTest
    {
        private TestHelper _th;


        private readonly string APP_KEY = ParameterHelper.GetPassword("dropbox_api_key");
        private readonly string REDIRECT_URI = ParameterHelper.GetPassword("dropbox_redirectUri");
        private readonly string DROPBOX_ACCESSTOKEN = ParameterHelper.GetPassword("dropbox_accesstoken");
        private const string DROPBOX_FOLDER = "DropboxTests";
        private DropboxClient _dropboxClient;

        private DropboxService _dropboxService;
        private List<string> _addedFiles;

        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _dropboxService = new DropboxService();
            _dropboxService.AccessToken = DROPBOX_ACCESSTOKEN;

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder(DROPBOX_FOLDER);
            _addedFiles = new List<string>();
            _dropboxClient = new DropboxClient(DROPBOX_ACCESSTOKEN);
        }

        [Test]
        public void Check_AuthorisationUriCreated_RetursnNonEmptyString()
        {
            var result = _dropboxService.GetAuthorizeUri(APP_KEY, REDIRECT_URI);
            // check is authentication url starts with redirecturi. if not test is failed.
            Assert.IsNotNullOrEmpty(result.AbsoluteUri);
        }

        [Test]
        public void GetAccessToken()
        {
            Assert.AreEqual(DROPBOX_ACCESSTOKEN, _dropboxService.GetDropUserInfo().AccessToken);
        }

        [Test]
        public void GetDropboxUserAccountId()
        {
            Assert.IsNotNullOrEmpty(_dropboxService.GetDropUserInfo().AccountId);
        }

        [Test]
        public void SetUserInfoAndCheckAccountInfo_ReturnsNonEmptyString()
        {
            Assert.IsNotNullOrEmpty(_dropboxService.GetDropUserInfo().AccountId);
        }

        [Test]
        public void UploadListOfFilesToDropboxFolder_NoUniqueFileNames_ReturnsTrue()
        {
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames);
            _addedFiles.AddRange(_th.Job.OutputFiles.Select(path => "/" + DROPBOX_FOLDER + "/" + Path.GetFileName(path)));

            Assert.IsTrue(result);
        }

        [Test]
        public void UploadListOfFilesToDropboxFolder_NoUniqueFileNames_CountBiggerThan0()
        {
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;
            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames);
            Assert.IsTrue(result);
        }


        [Test]
        public void UploadListOfFilesToDropboxFolderAndShareLink_NoUniqueFileNames_CountBiggerThan0()
        {
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames);
            _addedFiles.AddRange(result.Select(s => s.FilePath));

            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void UploadListOfFilesToDropboxFolderAndShareLink_UniqueFileNames_CountEquals2()
        {
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            var sameFileName = Guid.NewGuid();
            var file1Content = DateTime.Now.ToShortDateString() + " First file Content";
            var file2Content = DateTime.Now.ToShortDateString() + " Second file Content";
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, sameFileName + ".pdf", file1Content));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, sameFileName + ".pdf", file2Content));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = true;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames);
            _addedFiles.AddRange(result.Select(s => s.FilePath));

            Assert.AreEqual(result.Count, 2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NoDropboxAccessToken_ThrowsArgumentException()
        {
            _dropboxService.ParseAccessToken(new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver#state=eb3399f60f50429b94af8360ec377079&error_description=The+user+chose+not+to+give+your+app+access+to+their+Dropbox+account.&error=access_denied"));
        }

        [Test]
        public void UploadfFileToDropboxFolder_CheckSumOfUploadedFile_Check()
        {
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            var fileName = Guid.NewGuid() + ".pdf";
            var file1Content = DateTime.Now.ToShortDateString() + " First file Content";
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, fileName, file1Content));

            _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames);

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(_th.Job.OutputFiles.FirstOrDefault()))
                {
                    var uploadedFileCHeckSum = md5.ComputeHash(stream);
                    var downloadedFile = _dropboxClient.Files.DownloadAsync("/" + DROPBOX_FOLDER + "/" + fileName).Result;
                    var downloadedFileHash = md5.ComputeHash(downloadedFile.GetContentAsStreamAsync().Result);
                    Assert.AreEqual(uploadedFileCHeckSum, downloadedFileHash);
                }
            }
        }

        [Test]
        public void CanParseAccessToken_ReturnsNonEmptyString()
        {
            var url = $"https://www.dropbox.com/1/oauth2/redirect_receiver#access_token=" + DROPBOX_ACCESSTOKEN + "&token_type=bearer&state=1bc3f668b579483caeef4d177871144c&uid=601383411&account_id=dbid%3AAADDGW8GLuF6MWlqODbOkPRcT0GTpWakl8s";
            var result = _dropboxService.ParseAccessToken(new Uri(url));
            Assert.AreEqual(DROPBOX_ACCESSTOKEN, result);
        }

        [TearDown]
        public void CleanUp()
        {

            foreach (var file in _addedFiles)
            {
                _dropboxClient = new DropboxClient(DROPBOX_ACCESSTOKEN);
                _dropboxClient.Files.DeleteAsync(file).Wait();
            }
            _th.CleanUp();
        }
    }
}
