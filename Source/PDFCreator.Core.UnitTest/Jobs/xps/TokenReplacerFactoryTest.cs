using System.Collections.Generic;
using SystemInterface;
using SystemInterface.IO;
using SystemWrapper;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Jobs.xps;
using pdfforge.PDFCreator.Utilities.Tokens;
using PDFCreator.Core.UnitTest.Mocks;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Jobs.xps
{
    [TestFixture]
    internal class TokenReplacerFactoryTest
    {
        private IJobInfo _jobInfo;
        private TokenReplacerFactory _tokenReplacerFactory;
        private MockFileUtil _fileUtil;
        private IEnvironment _environment;
        private IJob _job;

        [SetUp]
        public void SetUp()
        {
            _jobInfo = new JobInfo();

            var sourceFileInfo = new SourceFileInfo();
            sourceFileInfo.ClientComputer = "someComputer";
            sourceFileInfo.JobCounter = 3;
            sourceFileInfo.JobId = 14;
            sourceFileInfo.PrinterName = "SomePrinter";
            sourceFileInfo.SessionId = 121;
            _jobInfo.SourceFiles.Add(sourceFileInfo);

            var metadata = new Metadata();
            metadata.PrintJobAuthor = "someAuthor";
            metadata.PrintJobName = "somePrintJobName";
            metadata.Author = "someAuthor";
            metadata.Title = "someTitle";
            _jobInfo.Metadata = metadata;

            _tokenReplacerFactory = new TokenReplacerFactory();

            _fileUtil = new MockFileUtil();
            _fileUtil.SetInstanceToMock();

            _environment = MockRepository.GenerateStub<IEnvironment>();
            _environment.Stub(e => e.UserName).Return("someUser");
            _environment.Stub(e => e.MachineName).Return("someMachine");

            var dateTime = MockRepository.GenerateStub<IDateTime>();
            dateTime.Stub(dt => dt.Now).Return(new DateTimeWrap(2000, 1, 1, 1, 1, 1));

            _job = MockRepository.GenerateStub<IJob>();
            _job.Stub(j => j.JobInfo).Return(_jobInfo);
            _job.Stub(j => j.NumberOfCopies).Return(5);
            _job.Stub(j => j.NumberOfPages).Return(14);

            _job.OutputFiles = new List<string>(){"outputFile", "test"};

            var path = MockRepository.GenerateStub<IPath>();
            path.Stub(p => p.GetFullPath("outputFile")).Return("fullPath");
            path.Stub(p => p.GetFileNameWithoutExtension("C:\\thedir\\thefile.txt")).Return("thefile");
            path.Stub(p => p.GetDirectoryName("C:\\thedir\\thefile.txt")).Return("C:\\thedir");
            path.Stub(p => p.GetFileName("")).IgnoreArguments().Return("file");

            _tokenReplacerFactory = new TokenReplacerFactory(dateTime, _environment, path);
        }

        [TestCase("ClientComputer", Result = "someComputer")]
        [TestCase("Counter", Result = "3")]
        [TestCase("JobId", Result = "14")]
        [TestCase("PrinterName", Result = "SomePrinter")]
        [TestCase("SessionId", Result = "121")]
        [TestCase("PrintJobAuthor", Result = "someAuthor")]
        [TestCase("PrintJobName", Result = "somePrintJobName")]
        [TestCase("Username", Result = "someUser")]
        [TestCase("ComputerName", Result = "someMachine")]
        [TestCase("Author", Result = "someAuthor")]
        [TestCase("Title", Result = "someTitle")]
        [TestCase("DateTime", Result = "01.01.2000 01:01:01")]
        public string BuildTokenReplacerFromJobInfo_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken(tokenName);
            return token.GetValue();
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfDocTitelIsNoPath_ReturnsTokenReplacerWithsomePrintJobNameAsInputFileName()
        {
            _jobInfo.SourceFiles[0].DocumentTitle = "titelThatIsNoPath";
            _fileUtil.RootedPath = false;

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("InputFilename");

            Assert.AreEqual("somePrintJobName", token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfDocTitelIsNoPath_ReturnsTokenReplacerWithEmptyInputFilePath()
        {
            _jobInfo.SourceFiles[0].DocumentTitle = "titelThatIsNoPath";
            _fileUtil.RootedPath = false;

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("InputFilePath");

            Assert.AreEqual("", token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfDocTitelIsPath_ReturnsTokenReplacerWithInputFileName()
        {
            _jobInfo.SourceFiles[0].DocumentTitle = "C:\\thedir\\thefile.txt";
            _fileUtil.RootedPath = true;

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("InputFilename");

            Assert.AreEqual("thefile", token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfDocTitelIsPath_ReturnsTokenReplacerWithInputFilePath()
        {
            _jobInfo.SourceFiles[0].DocumentTitle = "C:\\thedir\\thefile.txt";
            _fileUtil.RootedPath = true;

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("InputFilePath");

            Assert.AreEqual("C:\\thedir", token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_ReturnsTokenReplacerWithEnvironmentToken()
        {
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Environment");

            Assert.IsTrue(token is EnvironmentToken);
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfAuthorContainsTokens_ReturnsTokenReplacerWithCorrectAuthorToken()
        {
            _jobInfo.Metadata.Author = "<PrinterName>";
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Author");

            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfAuthorContainsTokens_ReturnsTokenReplacerWithCorrectTitleToken()
        {
            _jobInfo.Metadata.Title = "<PrinterName>";
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Title");

            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [TestCase("Author")]
        [TestCase("Title")]
        [TestCase("DateTime")]
        [TestCase("ComputerName")]
        [TestCase("Username")]
        [TestCase("Environment")]
        [TestCase("ClientComputer")]
        [TestCase("Counter")]
        [TestCase("JobId")]
        [TestCase("PrinterName")]
        [TestCase("SessionId")]
        [TestCase("PrintJobAuthor")]
        [TestCase("PrintJobName")]
        [TestCase("InputFilename")]
        [TestCase("InputFilePath")]
        public void BuildTokenReplacerWithoutOutputfiles_ReturnsTokenReplacerWithJobTokens(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(_job);
            var tokenList = tokenreplacer.GetTokenNames(false);

            Assert.Contains(tokenName, tokenList);
        }

        [TestCase("NumberOfCopies", Result = "5")]
        [TestCase("NumberOfPages", Result = "14")]
        public string BuildTokenReplacerWithoutOutputfiles_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(_job);
            var token = tokenreplacer.GetToken(tokenName);
            return token.GetValue();
        }

        [TestCase("OutputFilePath", Result = "fullPath")]
        [TestCase("OutputFilenames", Result = "file, file")]
        public string BuildTokenReplacerWithOutputfiles_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithOutputfiles(_job);
            var token = tokenreplacer.GetToken(tokenName);
            return token.GetValue();
        }
    }
}