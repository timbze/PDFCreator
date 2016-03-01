using System;
using System.IO;
using System.Net;
using FtpLib;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Actions;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities;

namespace PDFCreator.Core.IntegrationTest.Actions
{
    [TestFixture]
    [Category("LongRunning")]
    class FtpTest
    {
        private TestHelper _th;

        private readonly string _ftpServer = ParameterHelper.GetPassword("ftp_server");
        private readonly string _userName = ParameterHelper.GetPassword("ftp_user");
        private readonly string _password = ParameterHelper.GetPassword("ftp_password");

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("FtpTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void TestMultipleFileUploadInRoot()
        {
            ClearFtp();

            _th.Profile.Ftp.Enabled = true;
            _th.Profile.Ftp.Server = _ftpServer;
            _th.Profile.Ftp.Directory = "";
            _th.Profile.Ftp.UserName = _userName;
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);

            _th.Job.Passwords.FtpPassword = _password;

            var ftp = new FtpAction();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload(_th.Job.Profile.Ftp.Directory);
        }

        [Test]
        public void TestMultipleFileUploadInDirectory()
        {
            ClearFtp();

            _th.Profile.Ftp.Enabled = true;
            _th.Profile.Ftp.Server = _ftpServer;
            _th.Profile.Ftp.Directory = "testdirectory/abc";
            _th.Profile.Ftp.UserName = _userName;
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);

            _th.Job.Passwords.FtpPassword = _password;

            var ftp = new FtpAction();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload(_th.Job.Profile.Ftp.Directory);
        }

        [Test]
        public void TestFileUploadInDirectoryWithToken()
        {
            ClearFtp();

            _th.Profile.Ftp.Enabled = true;
            _th.Profile.Ftp.Server = _ftpServer;
            _th.Profile.Ftp.Directory = "testdirectory/<PrintJobAuthor>";
            _th.Profile.Ftp.UserName = _userName;
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _password;

            var ftp = new FtpAction();
            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload("testdirectory/SampleUser1234");
        }

        [Test]
        public void TestFileUploadInDirectoryWithTokensAndInvalidCharacters()
        {
            ClearFtp();
           
            _th.Profile.Ftp.Enabled = true;
            _th.Profile.Ftp.Server = _ftpServer;
            _th.Profile.Ftp.Directory = "testdirectory/<InvalidChars>";
            _th.Profile.Ftp.UserName = _userName;
            _th.Profile.Ftp.EnsureUniqueFilenames = true;

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Jpeg);
            _th.Job.Passwords.FtpPassword = _password;

            _th.Job.OutputFiles.Add(TempFileHelper.CreateTempFile("FtpTest", "test.pdf"));
            _th.Job.TokenReplacer.AddStringToken("InvalidChars", "Invalid|><:*?\"Chars");  

            var ftp = new FtpAction();
            ftp.ProcessJob(_th.Job);

            VerifyFileUpload("testdirectory/Invalid_Chars");
        }

        private void VerifyFileUpload(string ftpDirectory)
        {
            foreach (string file in _th.Job.OutputFiles)
            {   
                // Get the object used to communicate with the server.
                string downloadUrl = "ftp://" + _th.Profile.Ftp.Server + "/" + ftpDirectory + "/" + Path.GetFileName(file);
                var request = (FtpWebRequest)WebRequest.Create(new Uri(downloadUrl));
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(_th.Job.Profile.Ftp.UserName, _th.Job.Passwords.FtpPassword);

                var fi = new FileInfo(file);

                try
                {
                    var response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();

                    Assert.IsNotNull(responseStream);
                    var reader = new StreamReader(responseStream);
                    string content = reader.ReadToEnd();

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
            var ftp = new FtpConnection(_ftpServer, _userName, _password);
            ftp.Open();
            ftp.Login();

            ClearFtp(ftp, "/");

            ftp.Close();
        }

        public void ClearFtp(FtpConnection ftp, String directory)
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
        public void IsValidPath_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => FtpAction.IsValidPath(null));
        }

        [Test]
        public void IsValidPath_EmptyString_IsValid()
        {
            Assert.IsTrue(FtpAction.IsValidPath(""));
        }

        [Test]
        public void IsValidPath_RelativePaths_AreValid()
        {
            var validPaths = new[]
            {
                "abc",
                "abc/def",
                "abc/def.xyz",
                "abc/def/xyz ab ÖÄÜ",
                "abc//def",
                @"abc\def"
            };

            foreach (var validPath in validPaths)
            {
                Assert.IsTrue(FtpAction.IsValidPath(validPath), "\"" + validPath + "\" detected as invalid path.");
            }
        }

        [Test]
        public void IsValidPath_AbsolutePaths_AreValid()
        {
            var validPaths = new[]
            {
                "/abc",
                "/abc/def",
                "/abc/def.xyz",
                "/abc/def/xyz ab ÖÄÜ",
                @"\abc\def"
            };

            foreach (var validPath in validPaths)
            {
                Assert.IsTrue(FtpAction.IsValidPath(validPath), "unexpected: " + validPath);
            }
        }

        [Test]
        public void IsValidPath_InvalidPaths_AreInvalid()
        {
            var invalidPaths = new[]
            {
                "abc|",
                "path>",
                ">path",
                "<path",
                ":path",
                "*path",
                "?path",
                "\"path",
                "path/\\abc"
            };

            foreach (var invalidPath in invalidPaths)
            {
                Assert.IsFalse(FtpAction.IsValidPath(invalidPath), "unexpected: " + invalidPath);
            }
        }

        [Test]
        public void MakeValidPath_ValidPaths_AreUnchanged()
        {
            var validPaths = new[]
            {
                "/abc",
                "/abc/def",
                "/abc/def.xyz",
                "/abc/def/xyz ab ÖÄÜ",
                @"\abc\def"
            };

            foreach (var validPath in validPaths)
            {
                Assert.AreSame(validPath, FtpAction.MakeValidPath(validPath), "unexpected: " + validPath);
            }
        }

        [Test]
        public void MakeValidPath_InvalidPaths_AreValid()
        {
            var validPaths = new[]
            {
                "abc|",
                "path>",
                ">path",
                "<path",
                ":path",
                "*path",
                "?path",
                "\"path",
                "path/\\abc"
            };

            foreach (var validPath in validPaths)
            {
                Assert.IsTrue(FtpAction.IsValidPath(FtpAction.MakeValidPath(validPath)), "unexpected: " + validPath);
            }
        }

        [Test]
        public void MakeValidPath_ValidatedPaths_ContainUnderscore()
        {
            var validPaths = new[]
            {
                "abc|",
                "path>",
                ">path",
                "<path",
                ":path",
                "*path",
                "?path",
                "\"path"
            };

            foreach (var validPath in validPaths)
            {
                Assert.IsTrue(FtpAction.MakeValidPath(validPath).Contains("_"), "unexpected: " + validPath);
            }
        }
    }
}
