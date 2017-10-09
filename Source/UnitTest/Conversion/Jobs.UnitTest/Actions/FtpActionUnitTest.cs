using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static pdfforge.PDFCreator.Conversion.Jobs.ErrorCode;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class FtpActionUnitTest
    {
        [SetUp]
        public void Setup()
        {
            _ftpConnectionWrap = Substitute.For<IFtpConnection>();
            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.MAX_PATH.Returns(259);

            var factory = Substitute.For<IFtpConnectionFactory>();
            factory.BuildConnection(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(_ftpConnectionWrap).AndDoes(delegate (CallInfo info)
            {
                _host = info.ArgAt<string>(0);
                _userName = info.ArgAt<string>(1);
                _password = info.ArgAt<string>(2);
            });

            _ftpTestAccount = new FtpAccount();
            _ftpTestAccount.AccountId = "FtpTestAccountId";
            _ftpTestAccount.Server = ProfileServer;
            _ftpTestAccount.UserName = ProfileUserName;
            _ftpTestAccount.Password = "ProfileFtpPassword";

            _accounts = new Accounts();
            _accounts.FtpAccounts.Add(_ftpTestAccount);

            _profile = new ConversionProfile();
            _profile.Ftp.Enabled = true;
            _profile.Ftp.AccountId = _ftpTestAccount.AccountId;

            _ftpAction = new FtpAction(factory, _pathUtil);

            var jobPws = new JobPasswords();
            jobPws.FtpPassword = JobFtpPassword;
            _job = new Job(new JobInfo(), _profile, new JobTranslations(), _accounts);
            _job.Accounts = _accounts;
            _job.Passwords = jobPws;
            _job.TokenReplacer = new TokenReplacer();
        }

        private const string JobFtpPassword = "JobFtpPassword";
        private const string ProfileServer = "ProfileServer";
        private const string ProfileUserName = "ProfileUserName";

        private FtpAction _ftpAction;
        private IFtpConnection _ftpConnectionWrap;
        private IPathUtil _pathUtil;

        private Job _job;
        private ConversionProfile _profile;
        private Accounts _accounts;
        private FtpAccount _ftpTestAccount;

        private string _host;
        private string _userName;
        private string _password;

        [Test]
        public void ProcessInvalidJob_NoAccount_ResultContainsAssociatedCode()
        {
            _profile.Ftp.AccountId = "Some unavailble Account ID";

            var results = _ftpAction.ProcessJob(_job);

            Assert.AreEqual(Ftp_NoAccount, results[0]);
        }

        [Test]
        public void ProcessInvalidJob_AutoSaveNoPasswordSpecified_ResultContainsAssociatedCode()
        {
            _profile.AutoSave.Enabled = true;
            _ftpTestAccount.Password = "";
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_AutoSaveWithoutPassword, results[0]);
        }

        [Test]
        public void ProcessInvalidJob_NoJobFtpPassword_ResultIsTrue()
        {
            _job.Passwords = new JobPasswords();
            var results = _ftpAction.ProcessJob(_job); //The action deals with missing passwords and handels them itself...
            Assert.IsTrue(results);
        }

        [Test]
        public void ProcessInvalidJob_NoServerSpecified_ResultContainsAssociatedCode()
        {
            _ftpTestAccount.Server = "";
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_NoServer, results[0]);
        }

        [Test]
        public void ProcessInvalidJob_NoUserNameSpecified_ResultContainsAssociatedCode()
        {
            _ftpTestAccount.UserName = "";
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_NoUser, results[0]);
        }

        [Test]
        public void ProcessValidJob_CreateDirectoryThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.CreateDirectory("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_createFtpConnectionGetsCalledWithCorrectValues()
        {
            _ftpAction.ProcessJob(_job);
            Assert.AreEqual(ProfileServer, _host);
            Assert.AreEqual(ProfileUserName, _userName);
            Assert.AreEqual(JobFtpPassword, _password);
        }

        [Test]
        public void ProcessValidJob_CreatesNonExistingDirectory()
        {
            var folder = "Folder";
            _profile.Ftp.Directory = folder;
            _ftpConnectionWrap.DirectoryExists(Arg.Any<string>()).Returns(false);

            _ftpAction.ProcessJob(_job);

            Received.InOrder(() =>
            {
                _ftpConnectionWrap.CreateDirectory(folder);
                _ftpConnectionWrap.SetCurrentDirectory(folder);
            });
        }

        [Test]
        public void ProcessValidJob_CreatesNonExistingSubDirectories()
        {
            var folder = "Folder";
            var subFolder1 = "Subfolder1";
            var subFolder2 = "SubFolder2";
            _profile.Ftp.Directory = folder + @"/" + subFolder1 + @"/" + subFolder2;
            _ftpConnectionWrap.DirectoryExists(folder).Returns(true);
            _ftpConnectionWrap.DirectoryExists(subFolder1).Returns(false);
            _ftpConnectionWrap.DirectoryExists(subFolder1).Returns(false);

            _ftpAction.ProcessJob(_job);

            Received.InOrder(() =>
            {
                _ftpConnectionWrap.CreateDirectory(subFolder1);
                _ftpConnectionWrap.SetCurrentDirectory(subFolder1);
                _ftpConnectionWrap.CreateDirectory(subFolder2);
                _ftpConnectionWrap.SetCurrentDirectory(subFolder2);
            });
        }

        [Test]
        public void ProcessValidJob_DirectoryExistThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.DirectoryExists("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_DoesNotCreatesExistingDirectory()
        {
            var folder = "Folder";
            _profile.Ftp.Directory = folder;
            _ftpConnectionWrap.DirectoryExists(Arg.Any<string>()).Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Test]
        public void ProcessValidJob_FtpLoginCalledAfterOpen_EachOnlyOnce()
        {
            _ftpAction.ProcessJob(_job);
            _ftpConnectionWrap.Received(1).Open();
            _ftpConnectionWrap.Received(1).Login();
            Received.InOrder(() =>
            {
                _ftpConnectionWrap.Open();
                _ftpConnectionWrap.Login();
            });
        }

        [Test]
        public void ProcessValidJob_FtpOpenThrowsException_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_LoginError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpOpenThrowsWin32Exception_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_LoginError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void
            ProcessValidJob_FtpOpenThrowsWin32ExceptionNativeErrorCode12007_CallFtpClose_ResultContainsAssociatedCode()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(12007); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_ConnectionError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpPutFileThrowsExcpetion_CallFtpClose_ResultContainsAssociatedCode()
        {
            var outputFile = "file.pdf";
            _job.OutputFiles = new List<string>(new[] { outputFile });
            _ftpConnectionWrap.When(x => x.PutFile(Arg.Any<string>(), Arg.Any<string>()))
                .Do(x => { throw new Exception(); });

            var results = _ftpAction.ProcessJob(_job);

            Assert.AreEqual(Ftp_UploadError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_InvalidPathIsMadeValid()
        {
            var invalidPath = "Invalid " + new string(Path.GetInvalidPathChars()) + ":*? Path";
            _profile.Ftp.Directory = invalidPath;
            var validPath = "Invalid _ Path";

            _ftpAction.ProcessJob(_job);

            //Check first use of corrected path
            _ftpConnectionWrap.Received().DirectoryExists(validPath);
        }

        [Test]
        public void ProcessValidJob_MultipleOutputFiles_FtpPutFileGetsCalledMultipleTimes_CallFtpClose_ResultIstTrue()
        {
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });

            var results = _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, outputFile);
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, outputFile2);
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, outputFile3);

            Assert.IsTrue(results);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_SetCurrentDirectoryThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.Directory = "D";
            _ftpConnectionWrap.When(x => x.SetCurrentDirectory("D")).Do(x => { throw new Exception(); });
            var results = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(Ftp_DirectoryError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_TokenInDirectoryGetReplaced()
        {
            var tr = new TokenReplacer();
            tr.AddToken(new StringToken("token", "sometokenvalue"));
            _job.TokenReplacer = tr;

            _profile.Ftp.Directory = "<token>";
            var validPath = "sometokenvalue";

            _ftpAction.ProcessJob(_job);

            //Check first use of corrected path
            _ftpConnectionWrap.Received().DirectoryExists(validPath);
        }

        [Test]
        public void ProcessValidJobEnsureUniqueFileNames_FirstFileDoesExist_FilesGetAppendix()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });
            _ftpConnectionWrap.FileExists(outputFile).Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, "file_2.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, "file2.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, "file3.jpg");
        }

        [Test]
        public void ProcessValidJobEnsureUniqueFileNames_FirstFileDoesExistFileWithAppendixDoesExist_FilesGetIncreasedAppendix()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.jpg";
            var outputFile2 = "file2.jpg";
            var outputFile3 = "file3.jpg";
            _job.OutputFiles = new List<string>(new[] { outputFile, outputFile2, outputFile3 });
            _ftpConnectionWrap.FileExists(outputFile2).Returns(true);
            _ftpConnectionWrap.FileExists("file2_2.jpg").Returns(true);

            _ftpAction.ProcessJob(_job);

            _ftpConnectionWrap.Received(1).PutFile(outputFile, "file.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile2, "file2_3.jpg");
            _ftpConnectionWrap.Received(1).PutFile(outputFile3, "file3.jpg");
        }

        [Test]
        public void
            ProcessValidJobEnsureUniqueFilenamesFtpFileExistsThrowsExpection_CallFtpClose_ResultContainsAssociatedCode()
        {
            _profile.Ftp.EnsureUniqueFilenames = true;
            var outputFile = "file.pdf";
            _job.OutputFiles = new List<string>(new[] { outputFile });

            _ftpConnectionWrap.When(x => x.FileExists(Arg.Any<string>())).Do(x => { throw new Exception(); });

            var results = _ftpAction.ProcessJob(_job);

            Assert.AreEqual(Ftp_DirectoryReadError, results[0]);
            _ftpConnectionWrap.Received(1).Close();
        }

        [Test]
        public void ProcessValidJob_FtpOpenThrowsWin32ExceptionNativeErrorCode12014_CallFtpClose_ResultsAreEmpty()
        {
            _ftpConnectionWrap.When(x => x.Open()).Do(x => { throw new Win32Exception(12014); });
            var result = _ftpAction.ProcessJob(_job);
            Assert.AreEqual(0, result.Count);
            _ftpConnectionWrap.Received(1).Close();
        }

        #region actioncheck

        [Test]
        public void FTPSettings_Autosave_valid()
        {
            _profile.Ftp.Enabled = true;
            SetValidAutoSaveSettings();

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_Autosave_MultipleErrors()   //TODO move these tests to FtpActionTest
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "";
            _ftpTestAccount.Server = "";
            _ftpTestAccount.UserName = "";
            _ftpTestAccount.Password = "";
            SetValidAutoSaveSettings();

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.Ftp_NoServer, result, "ProfileCheck did not detect missing FTP server.");
            result.Remove(ErrorCode.Ftp_NoServer);
            Assert.Contains(ErrorCode.Ftp_NoUser, result, "ProfileCheck did not detect missing FTP username.");
            result.Remove(ErrorCode.Ftp_NoUser);
            Assert.Contains(ErrorCode.Ftp_AutoSaveWithoutPassword, result, "ProfileCheck did not detect missing FTP password for autosave.");
            result.Remove(ErrorCode.Ftp_AutoSaveWithoutPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_Autosave_NoPassword()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _ftpTestAccount.Server = "random ftp server";
            _ftpTestAccount.UserName = "random ftp username";
            _ftpTestAccount.Password = "";
            SetValidAutoSaveSettings();

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.Ftp_AutoSaveWithoutPassword, result, "ProfileCheck did not detect missing FTP password for autosave.");
            result.Remove(ErrorCode.Ftp_AutoSaveWithoutPassword);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_NoAutosave_MultipleErrors()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "";
            _ftpTestAccount.Server = "";
            _ftpTestAccount.UserName = "";
            _ftpTestAccount.Password = "";
            _profile.AutoSave.Enabled = false;

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.Ftp_NoServer, result, "ProfileCheck did not detect missing FTP server.");
            result.Remove(ErrorCode.Ftp_NoServer);
            Assert.Contains(ErrorCode.Ftp_NoUser, result, "ProfileCheck did not detect missing FTP username.");
            result.Remove(ErrorCode.Ftp_NoUser);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_NoAutosave_NoServer()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _ftpTestAccount.Server = "";
            _ftpTestAccount.UserName = "random ftp username";
            _ftpTestAccount.Password = "";
            _profile.AutoSave.Enabled = false;

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.Ftp_NoServer, result, "ProfileCheck did not detect missing FTP server.");
            result.Remove(ErrorCode.Ftp_NoServer);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_NoAutosave_NoUsername()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _ftpTestAccount.Server = "random ftp server";
            _ftpTestAccount.UserName = "";
            _ftpTestAccount.Password = "";
            _profile.AutoSave.Enabled = false;

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.Ftp_NoUser, result, "ProfileCheck did not detect missing FTP username.");
            result.Remove(ErrorCode.Ftp_NoUser);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void FTPSettings_NoAutosave_valid()
        {
            _profile.Ftp.Enabled = true;
            _profile.Ftp.Directory = "random ftp directory";
            _ftpTestAccount.Server = "random ftp server";
            _ftpTestAccount.UserName = "random ftp username";
            _ftpTestAccount.Password = "";
            _profile.AutoSave.Enabled = false;

            var result = _ftpAction.Check(_profile, _accounts);

            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        private void SetValidAutoSaveSettings()
        {
            _profile.AutoSave.Enabled = true;
            _profile.TargetDirectory = "random autosave directory";
            _profile.FileNameTemplate = "random autosave filename";
        }

        #endregion actioncheck
    }
}
