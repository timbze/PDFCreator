using FtpLib;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using SimpleInjector;
using System;
using System.IO;
using System.Net;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    [Category("Brittle")]
    internal class FtpTest
    {
        private TestHelper _th;
        private FtpAccount _ftpTestAccount;
        private Accounts _accounts;

        [SetUp]
        public void SetUp()
        {
            _ftpTestAccount = new FtpAccount();
            _ftpTestAccount.AccountId = "FtpTestAccountID";
            _ftpTestAccount.Server = ParameterHelper.GetPassword("ftp_server");
            _ftpTestAccount.UserName = ParameterHelper.GetPassword("ftp_user");
            _ftpTestAccount.Password = ParameterHelper.GetPassword("ftp_password");

            var bootstrapper = new IntegrationTestBootstrapper();

            _container = bootstrapper.ConfigureContainer();
            _container.Options.AllowOverridingRegistrations = true;
            _container.Options.AllowOverridingRegistrations = false;

            _th = _container.GetInstance<TestHelper>();
            _th.InitTempFolder("FtpTest");

            _th.Profile.Ftp.Enabled = true;
            _th.Profile.Ftp.AccountId = _ftpTestAccount.AccountId;

            _accounts = new Accounts();
            _accounts.FtpAccounts.Add(_ftpTestAccount);
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private Container _container;

        private void VerifyFileUpload(string ftpDirectory)
        {
            foreach (var file in _th.Job.OutputFiles)
            {
                // Get the object used to communicate with the server.
                var downloadUrl = "ftp://" + _ftpTestAccount.Server + "/" + ftpDirectory + "/" + Path.GetFileName(file);
                var request = (FtpWebRequest)WebRequest.Create(new Uri(downloadUrl));
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(_ftpTestAccount.UserName, _ftpTestAccount.Password);

                var fi = new FileInfo(file);

                try
                {
                    var response = (FtpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();

                    Assert.IsNotNull(responseStream);
                    var reader = new StreamReader(responseStream);
                    var content = reader.ReadToEnd();

                    Assert.AreEqual(fi.Length, content.Length);
                    reader.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.GetType() + " while downloading the file " + downloadUrl + "\r\n" + ex.Message);
                }
            }
        }

        private void ClearFtp()
        {
            var ftp = new FtpConnection(_ftpTestAccount.Server, _ftpTestAccount.UserName, _ftpTestAccount.Password);
            ftp.Open();
            ftp.Login();

            ClearFtp(ftp, "/");

            ftp.Close();
        }

        public void ClearFtp(FtpConnection ftp, string directory)
        {
            ftp.SetCurrentDirectory(directory);
            var dirs = ftp.GetDirectories();
            foreach (var dir in dirs)
            {
                if ((dir.Name != ".") && (dir.Name != ".."))
                {
                    ClearFtp(ftp, dir.Name); //Recursive call
                    ftp.RemoveDirectory(dir.Name);
                }
            }

            foreach (var file in ftp.GetFiles())
            {
                ftp.RemoveFile(file.Name);
            }

            if (ftp.GetCurrentDirectory() != "/")
                ftp.SetCurrentDirectory("..");
        }

        [Test]
        public void TestFileUploadInDirectoryWithToken()
        {
            ClearFtp();

            _th.Profile.Ftp.Directory = "testdirectory/<PrintJobAuthor>";
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _ftpTestAccount.Password;
            _th.Job.Accounts = _accounts;

            var ftp = _container.GetInstance<FtpAction>();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload("testdirectory/SampleUser1234");
        }

        [Test]
        public void TestFileUploadInDirectoryWithTokensAndInvalidCharacters()
        {
            ClearFtp();

            _th.Profile.Ftp.Directory = "testdirectory/<InvalidChars>";
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _ftpTestAccount.Password;
            _th.Job.Accounts = _accounts;

            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.TokenReplacer.AddStringToken("InvalidChars", "Invalid|><:*?\"Chars");

            var ftp = _container.GetInstance<FtpAction>();
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload("testdirectory/Invalid_Chars");
        }

        [Test]
        public void TestMultipleFileUploadInDirectory()
        {
            ClearFtp();

            _th.Profile.Ftp.Directory = "testdirectory/abc";
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _ftpTestAccount.Password;
            _th.Job.Accounts = _accounts;

            var ftp = _container.GetInstance<FtpAction>();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload(_th.Job.Profile.Ftp.Directory);
        }

        [Test]
        public void TestMultipleFileUploadInRoot()
        {
            ClearFtp();

            _th.Profile.Ftp.Directory = "";
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _ftpTestAccount.Password;
            _th.Job.Accounts = _accounts;

            var ftp = _container.GetInstance<FtpAction>();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload(_th.Job.Profile.Ftp.Directory);
        }
    }
}
