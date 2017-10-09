using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class InteractiveOutputFileMoverTest
    {
        private Job _job;
        private string[] _singleTempOutputfile;
        private string[] _multipleTempOutputFiles;
        private string[] _multipleTempOutputFilesWithTwoDigits;
        private IRetypeFileNameQuery _retypeQuery;

        private const string RetypedFilename = "C:\\retype_";

        [SetUp]
        public void Setup()
        {
            var jobInfo = new JobInfo();
            _job = new Job(jobInfo, new ConversionProfile(), new JobTranslations(), new Accounts());

            _singleTempOutputfile = new[] { @"output1.pdf" };
            _multipleTempOutputFiles = new[] { @"output1.png", @"output2.png", @"output3.png" };
            _multipleTempOutputFilesWithTwoDigits = new[]
            {
                @"output1.png", @"output2.png", @"output3.png",
                @"output4.png", @"output5.png", @"output6.png",
                @"output7.png", @"output8.png", @"output9.png",
                @"output10.png"
            };

            _retypeQuery = Substitute.For<IRetypeFileNameQuery>();
        }

        [Test]
        public void FirstAttemptToCopyFirstFileFails_OnRetypeOutputFilenameGetsCalled_CancelInRetypeFilename_JobOutFilesAreEmpty()
        {
            var fileStub = Substitute.For<IFile>();
            fileStub
                .When(x => x.Copy(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()))
                .Do(x => { throw new IOException(); });

            _retypeQuery.RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>()).Returns(new QueryResult<string>(false, null));

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            _retypeQuery.RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>()).Returns(new QueryResult<string>(false, null));

            Assert.Throws<AbortWorkflowException>(() => outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was not called exactly once");
            Assert.IsEmpty(_job.OutputFiles, "Outputfiles are not empty.");
        }

        [Test]
        public void FirstAttemptToCopyThirdFileFails_OnRetypeOutputFilenameWasNotCalled()
        {
            int copyCount = 0;
            var fileStub = Substitute.For<IFile>();
            fileStub
                .When(x => x.Copy(_multipleTempOutputFiles[2], Arg.Any<string>(), Arg.Any<bool>()))
                .Do(x =>
                {
                    if (copyCount++ == 0)
                        throw new IOException();
                });

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(0, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called.");
        }

        [Test]
        public void CopyThirdFileFailsTwice_ThrowsException()
        {
            var fileStub = Substitute.For<IFile>();
            fileStub
                .When(x => x.Copy(_multipleTempOutputFiles[2], Arg.Any<string>(), Arg.Any<bool>()))
                .Do(x =>
                {
                    throw new IOException();
                });

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            Assert.Throws<ProcessingException>(() => outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(0, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called.");
        }

        [Test]
        public void MoveOutFiles_MultipleFilesInteractive_FirstThreeAttemptsToCopyFirstFileFail_OnRetypeOutputFilenameGetsCalledTriple_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            var fileStub = new FailingFileCopy(3);
            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Png;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(3, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual($"{RetypedFilename}3.png", _job.OutputFilenameTemplate,
                "OutputFilenameTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "1" + ".png", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "2" + ".png", _job.OutputFiles[1],
                "Second outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "3" + ".png", _job.OutputFiles[2],
                "Third outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void FirstAttemptToCopyFirstFileFails_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfiles()
        {
            var fileStub = new FailingFileCopy(1);

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Png;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "1.png", _job.OutputFilenameTemplate,
                "OutputFilenameTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "1" + ".png", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "2" + ".png", _job.OutputFiles[1],
                "Second outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "3" + ".png", _job.OutputFiles[2],
                "Third outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void SingleFile_FirstAttemptToCopyFails_OnRetypeOutputFilenameGetsCalled_CancelInRetypeFilename_JobOutFilesAreEmpty()
        {
            var fileStub = new FailingFileCopy(1);
            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;

            _retypeQuery.RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>()).Returns(new QueryResult<string>(false, null));

            _job.TempOutputFiles = _singleTempOutputfile;

            Assert.Throws<AbortWorkflowException>(() => outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.IsEmpty(_job.OutputFiles, "Outputfiles are not empty.");
        }

        [Test]
        public void SingleFile_FirstAttemptToCopyFails_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            var fileStub = new FailingFileCopy(1);

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;

            _job.TempOutputFiles = _singleTempOutputfile;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "1.pdf", _job.OutputFilenameTemplate,
                "OutputFilenameTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + ".pdf", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void SingleFile_ThreeAttemptsToCopyFail_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            var fileStub = new FailingFileCopy(3);

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;

            _job.TempOutputFiles = _singleTempOutputfile;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(3, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "3.pdf", _job.OutputFilenameTemplate,
                "OutputFilenameTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "3" + ".pdf", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void OnRetry_CallsDispatcherToQuery()
        {
            var fileStub = new FailingFileCopy(3);
            var dispatcher = Substitute.For<IDispatcher>();
            dispatcher.InvokeAsync<QueryResult<string>>(null).ReturnsForAnyArgs(x => Task.Run(x.Arg<Func<QueryResult<string>>>()));

            var outputFileMover = BuildInteractiveFileMover(fileStub, dispatcher);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;

            _job.TempOutputFiles = _singleTempOutputfile;

            outputFileMover.MoveOutputFiles(_job);

            dispatcher.ReceivedWithAnyArgs().InvokeAsync<QueryResult<string>>(null);
        }

        [Test]
        public void DeleteFileFails_DoesNotThrowException()
        {
            var fileStub = Substitute.For<IFile>();
            fileStub
                .When(x => x.Delete(Arg.Any<string>()))
                .Do(x => { throw new IOException(); });

            _retypeQuery.RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>()).Returns(new QueryResult<string>(false, null));

            var outputFileMover = BuildInteractiveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            _retypeQuery.RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>()).Returns(new QueryResult<string>(false, null));

            Assert.DoesNotThrow(() => outputFileMover.MoveOutputFiles(_job));
        }

        private InteractiveOutputFileMover BuildInteractiveFileMover(IFile file, IDispatcher dispatcher = null)
        {
            if (dispatcher == null)
                dispatcher = new InvokeImmediatelyDispatcher();

            var pathUtil = Substitute.For<IPathUtil>();

            int retypeCount = 0;
            _retypeQuery
                .RetypeFileName(Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(x => new QueryResult<string>(true, $"{RetypedFilename}{++retypeCount}." + _job.Profile.OutputFormat.ToString().ToLower()));

            return new InteractiveOutputFileMover(Substitute.For<IDirectory>(), file, pathUtil, _retypeQuery, dispatcher);
        }
    }
}
