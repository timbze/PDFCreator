using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    internal class AutoSaveOutputFileMoverTest
    {
        private Job _job;
        private string[] _singleTempOutputfile;
        private string[] _multipleTempOutputFiles;
        private string[] _multipleTempOutputFilesWithTwoDigits;

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
        }

        [Test]
        public void SingleFile_WhenOutputFileExists_UsesEnsureUniqueFilename()
        {
            var outputFile = _singleTempOutputfile[0];

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists(outputFile).Returns(true);
            fileStub.Exists(Arg.Is<string>(x => x != outputFile)).Returns(false);

            var outputFileMover = BuildAutosaveFileMover(fileStub);
            _job.Profile.AutoSave.Enabled = true;
            _job.Profile.AutoSave.EnsureUniqueFilenames = true;

            _job.TempOutputFiles = _singleTempOutputfile;

            outputFileMover.MoveOutputFiles(_job);

            _job.TempOutputFiles = _singleTempOutputfile;

            outputFileMover.MoveOutputFiles(_job);

            Assert.AreNotEqual(outputFile, _job.OutputFiles[0], "EnsureUniqueFilename was not applied");
        }

        private AutosaveOutputFileMover BuildAutosaveFileMover(IFile file)
        {
            var pathUtil = Substitute.For<IPathUtil>();

            return new AutosaveOutputFileMover(Substitute.For<IDirectory>(), file, pathUtil);
        }
    }
}
