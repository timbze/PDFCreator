using Dropbox.Api;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Dropbox;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    [Category("Brittle")]
    public class DropboxServiceTest
    {
        private TestHelper _th;

        private readonly string APP_KEY = ParameterHelper.GetPassword("dropbox_api_key");
        private readonly string REDIRECT_URI = ParameterHelper.GetPassword("dropbox_redirectUri");
        private readonly string DROPBOX_ACCESSTOKEN = ParameterHelper.GetPassword("dropbox_accesstoken");
        private string BASE_FOLDER_NAME = "AdditionalFolder";
        private DropboxClient _dropboxClient;

        private DropboxService _dropboxService;
        private List<string> _addedFiles;
        private List<string> _addedFolders;

        [SetUp]
        public void SetUp()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _dropboxService = new DropboxService();

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder(DROPBOX_FOLDER);
            _addedFiles = new List<string>();
            _addedFolders = new List<string>();
            _dropboxClient = new DropboxClient(DROPBOX_ACCESSTOKEN);
        }

        [TearDown]
        public void CleanUp()
        {
            _dropboxClient = new DropboxClient(DROPBOX_ACCESSTOKEN);

            foreach (var file in _addedFiles.Distinct())
            {
                _dropboxClient.Files.DeleteAsync(file).Wait();
            }

            foreach (var file in _addedFolders.Distinct())
            {
                _dropboxClient.Files.DeleteAsync("/" + file).Wait();
            }

            _th.CleanUp();
        }

        #region General tests

        [Test]
        public void NoDropboxAccessToken_ThrowsArgumentException()
        {
            var uri = new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver#state=eb3399f60f50429b94af8360ec377079&error_description=The+user+chose+not+to+give+your+app+access+to+their+Dropbox+account.&error=access_denied");
            Assert.Throws<ArgumentException>(() => _dropboxService.ParseAccessToken(uri));
        }

        [Test]
        public void CanParseAccessToken_ReturnsNonEmptyString()
        {
            var url = "https://www.dropbox.com/1/oauth2/redirect_receiver#access_token=" + DROPBOX_ACCESSTOKEN + "&token_type=bearer&state=1bc3f668b579483caeef4d177871144c&uid=601383411&account_id=dbid%3AAADDGW8GLuF6MWlqODbOkPRcT0GTpWakl8s";
            var result = _dropboxService.ParseAccessToken(new Uri(url));
            Assert.AreEqual(DROPBOX_ACCESSTOKEN, result);
        }

        [Test]
        public void Check_AuthorisationUriCreated_RetursnNonEmptyString()
        {
            var result = _dropboxService.GetAuthorizeUri(APP_KEY, REDIRECT_URI);
            // check is authentication url starts with redirecturi. if not test is failed.
            Assert.IsNotNull(result.AbsoluteUri);
            Assert.IsNotEmpty(result.AbsoluteUri);
        }

        [Test]
        public void GetAccessToken()
        {
            Assert.AreEqual(DROPBOX_ACCESSTOKEN, _dropboxService.GetDropUserInfo(DROPBOX_ACCESSTOKEN).AccessToken);
        }

        [Test]
        public void SetUserInfoAndCheckAccountInfo_ReturnsNonEmptyString()
        {
            var userInfo = _dropboxService.GetDropUserInfo(DROPBOX_ACCESSTOKEN).AccountId;
            Assert.IsNotNull(userInfo);
            Assert.IsNotEmpty(userInfo);
        }

        #endregion General tests

        #region Upload and sharing files

        [Test]
        public void UploadOneFilesToDropboxAndShareLink_WhenNoAdditionalFolderProvided_CountEqueals1PathIsRoot()
        {
            var DROPBOX_FOLDER = string.Empty;
            BASE_FOLDER_NAME = string.Empty;
            var tempFileName = Guid.NewGuid() + ".pdf";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);

            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, tempFileName));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);
            _addedFiles.Add(result.Filename);
            Assert.AreEqual("/" + tempFileName, result.Filename);
            Assert.IsNotNull(result);
        }

        [Test]
        public void UploadOneFilesToDropboxFolderAndShareLink_WhenAdditinalFolderProvided_CountEquealsOnePathIsRoot()
        {
            var DROPBOX_FOLDER = "DropboxSharedFolders";
            var tempFileName = Guid.NewGuid() + ".pdf";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);

            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, tempFileName));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);
            _addedFolders.Add(DROPBOX_FOLDER);

            StringAssert.AreEqualIgnoringCase("/" + DROPBOX_FOLDER + "/" + tempFileName, result.Filename);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Upload2FilesToDropboxRootFolderAndShareLink_WhenNoAdditionalFolderProvided_FolderIsShared()
        {
            var DROPBOX_FOLDER = string.Empty;
            var file1 = Guid.NewGuid() + ".pdf";
            var file2 = Guid.NewGuid() + ".pdf";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, file1));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, file2));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);
            _addedFolders.Add(BASE_FOLDER_NAME);

            StringAssert.AreEqualIgnoringCase("/" + BASE_FOLDER_NAME, result.Filename);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Upload2FilesToDropboxAdditionalFolderAndShareLink_WhenNoAdditionalFolderProvided_AdditionalFolderIsShared()
        {
            var DROPBOX_FOLDER = string.Empty;
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

            _addedFolders.Add(BASE_FOLDER_NAME);
            Assert.IsNotNull(result);
            StringAssert.AreEqualIgnoringCase("/" + BASE_FOLDER_NAME, result.Filename);
        }

        [Test]
        public void Upload2FilesToDropboxAdditionalFolderAndShareLink_WhenAdditionalFolderProvided_AdditionalFolderIsShared()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

            _addedFolders.Add(DROPBOX_FOLDER);
            Assert.IsNotNull(result);
            StringAssert.AreEqualIgnoringCase("/" + DROPBOX_FOLDER + "/" + BASE_FOLDER_NAME, result.Filename);
        }

        [Test]
        public void UploadListOfFilesToDropboxFolderAndShareLink_UniqueFileNames_CountEquals2()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            var sameFileName = Guid.NewGuid();
            var file1Content = DateTime.Now.ToShortDateString() + " First file Content";
            var file2Content = DateTime.Now.ToShortDateString() + " Second file Content";
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, sameFileName + ".pdf", file1Content));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, sameFileName + ".pdf", file2Content));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = true;

            var result = _dropboxService.UploadFileWithSharing(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);
            _addedFolders.Add(DROPBOX_FOLDER);
            Assert.IsNotNull(result);
        }

        #endregion Upload and sharing files

        #region Only uploading files

        [Test]
        public void UploadOneFilesToDropbox_WhenNoAdditionalFolderProvided_ReturnsTrue()
        {
            var DROPBOX_FOLDER = string.Empty;
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;
            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);
            _addedFiles.AddRange(_th.Job.OutputFiles.Select(path => "/" + Path.GetFileName(path)));
            Assert.IsTrue(result);
        }

        [Test]
        public void Upload2FilesToDropboxFolder_WhenDropboxFolderDefined_ReturnsTrue()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;
            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

            _addedFolders.Add(DROPBOX_FOLDER);
            Assert.IsTrue(result);
        }

        [Test]
        public void UploadListOfFilesToDropboxFolder_NoUniqueFileNames_CountBiggerThan0()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;
            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

            _addedFolders.Add(DROPBOX_FOLDER);
            Assert.IsTrue(result);
        }

        [Test]
        public void UploadfFileToRoot_CheckSumOfUploadedFileAndPathOfFile_Check()
        {
            var DROPBOX_FOLDER = "DropboxTestsUploadingFiles";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            var fileName = Guid.NewGuid() + ".pdf";
            var file1Content = DateTime.Now.ToShortDateString() + " First file Content";
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, fileName, file1Content));

            _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

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
            _addedFolders.Add(DROPBOX_FOLDER);
        }

        [Test]
        public void UploadfFileToDropboxFolder_CheckSumOfUploadedFile_Check()
        {
            var DROPBOX_FOLDER = "DropboxTestsUploadingFiles";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            var fileName = Guid.NewGuid() + ".pdf";
            var file1Content = DateTime.Now.ToShortDateString() + " First file Content";
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, fileName, file1Content));
            _addedFolders.Add(DROPBOX_FOLDER);

            _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

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
        public void UploadListOfFilesToDropboxFolder_NoUniqueFileNames_ReturnsTrue()
        {
            string DROPBOX_FOLDER = "DropboxTests";
            _th.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile(DROPBOX_FOLDER, Guid.NewGuid() + ".pdf"));
            _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames = false;

            var result = _dropboxService.UploadFiles(DROPBOX_ACCESSTOKEN, DROPBOX_FOLDER, _th.Job.OutputFiles, _th.Job.Profile.DropboxSettings.EnsureUniqueFilenames, BASE_FOLDER_NAME);

            _addedFolders.Add(DROPBOX_FOLDER);
            Assert.IsTrue(result);
        }

        #endregion Only uploading files
    }
}
