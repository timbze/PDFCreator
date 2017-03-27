using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class JobCleanUpTest
    {
        [SetUp]
        public void SetUp()
        {
            _directory = Substitute.For<IDirectory>();
            _directory.Exists("").ReturnsForAnyArgs(true);
            _file = Substitute.For<IFile>();

            _infFile = @"D:\test\test\something.inf";
            _jobTempFolder = @"D:\test\tempFolder";

            _jobCleanUp = new JobCleanUp(_directory, _file);
        }

        private IDirectory _directory;
        private IFile _file;
        private JobCleanUp _jobCleanUp;
        private string _infFile;
        private string _jobTempFolder;

        [Test]
        public void DeleteInfFile_DeletesInfFile()
        {
            _jobCleanUp.DoCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile);

            _file.Received().Delete(_infFile);
        }

        [Test]
        public void DeleteInfFile_IfDeleteThrowsExcepton_CatchesException()
        {
            _file.When(x => x.Delete(_infFile)).Do(x => { throw new IOException(); });

            Assert.DoesNotThrow(() => _jobCleanUp.DoCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile));
        }

        [Test]
        public void DeleteSourceFiles_DeletesSourceFiles()
        {
            var sourceFileName = @"D:\test\Spool\fileName.sourcefile";
            var sourceFiles = new List<SourceFileInfo> {new SourceFileInfo {Filename = sourceFileName}};

            _file.Exists(sourceFileName).Returns(true);

            _jobCleanUp.DoCleanUp(_jobTempFolder, sourceFiles, _infFile);

            _file.Received().Delete(sourceFileName);
        }

        [Test]
        public void DeleteSourceFiles_IfDeleteThrowsExcepton_CatchesException()
        {
            var fileName = @"D:\test\Spool\fileName.sourcefile";
            var sourceFiles = new List<SourceFileInfo> {new SourceFileInfo {Filename = fileName}};

            _file.When(x => x.Delete(fileName)).Do(x => { throw new IOException(); });

            Assert.DoesNotThrow(() => _jobCleanUp.DoCleanUp(_jobTempFolder, sourceFiles, _infFile));
        }

        [Test]
        public void DeleteTemporaryOutput_DeletesTemporaryOutputFiles()
        {
            _directory.Exists(_jobTempFolder).Returns(true);

            _jobCleanUp.DoCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile);
            _directory.Received().Delete(_jobTempFolder, true);
        }

        [Test]
        public void DeleteTemporaryOutput_IfDeleteThrowsExcepton_CatchesException()
        {
            _directory.Exists(_jobTempFolder).Returns(true);
            _directory.When(x => x.Delete(_jobTempFolder)).Do(x => { throw new IOException(); });

            Assert.DoesNotThrow(() => _jobCleanUp.DoCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile));
        }

        [Test]
        public void DeleteTemporaryOutput_IfJobTempFolderDoesNotExist_DoesNotCallDelete()
        {
            _directory.Exists(_jobTempFolder).Returns(false);

            _jobCleanUp.DoCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile);

            _directory.DidNotReceive().Delete(_jobTempFolder, true);
        }

        //TODO Delete folder if empty and not spool
    }
}