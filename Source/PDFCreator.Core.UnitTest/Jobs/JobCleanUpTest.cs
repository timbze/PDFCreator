using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;
using PDFCreator.Core.UnitTest.Mocks;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Jobs
{
    [TestFixture]
    public class JobCleanUpTest
    {
        private IDirectory _directory;
        private IFile _file;
        private JobCleanUp _jobCleanUp;
        private string _infFile;
        private string _jobTempFolder;

        [SetUp]
        public void SetUp()
        {
            _directory = MockRepository.GenerateStub<IDirectory>();
            _file = MockRepository.GenerateStub<IFile>();

            _infFile = @"D:\test\test\something.inf";
            _jobTempFolder = @"D:\test\tempFolder";
            
            _jobCleanUp = new JobCleanUp(_jobTempFolder, new List<SourceFileInfo>(), _infFile);
            _jobCleanUp.Directory = _directory;
            _jobCleanUp.File = _file;

            var fileUtil = new MockFileUtil();
            fileUtil.SetInstanceToMock();
        }

        [Test]
        public void DeleteTemporaryOutput_DeletesTemporaryOutputFiles()
        {
            _directory.Stub(dir => dir.Exists(_jobTempFolder)).Return(true);

            _jobCleanUp.DoCleanUp();
            _directory.AssertWasCalled(dir => dir.Delete(_jobTempFolder, true));
        }

        [Test]
        public void DeleteTemporaryOutput_IfDeleteThrowsExcepton_CatchesException()
        {
            _directory.Stub(dir => dir.Exists(_jobTempFolder)).Return(true);
            _directory.Stub(dir => dir.Delete(_jobTempFolder, true)).Throw(new IOException());

            Assert.DoesNotThrow(() => _jobCleanUp.DoCleanUp());
        }

        [Test]
        public void DeleteTemporaryOutput_IfJobTempFolderDoesNotExist_DoesNotCallDelete()
        {
            _directory.Stub(dir => dir.Exists(_jobTempFolder)).Return(false);

            _jobCleanUp.DoCleanUp();

            _directory.AssertWasNotCalled(dir => dir.Delete(_jobTempFolder, true));
        }

        [Test]
        public void DeleteSourceFiles_DeletesSourceFiles()
        {
            var sourceFileName = @"D:\test\Spool\fileName.sourcefile";

            var sourceFiles = new List<SourceFileInfo> { new SourceFileInfo { Filename = sourceFileName } };

            var jobCleanUp = new JobCleanUp(_jobTempFolder, sourceFiles, _infFile);
            jobCleanUp.Directory = _directory;
            jobCleanUp.File = _file;

            jobCleanUp.DoCleanUp();

            _file.AssertWasCalled(dir => dir.Delete(sourceFileName));
        }

        [Test]
        public void DeleteSourceFiles_IfDeleteThrowsExcepton_CatchesException()
        {

            var fileName = @"D:\test\Spool\fileName.sourcefile";
            var sourceFiles = new List<SourceFileInfo> { new SourceFileInfo { Filename = fileName } };
            var jobCleanUp = new JobCleanUp(_jobTempFolder, sourceFiles, _infFile);
            jobCleanUp.Directory = _directory;
            jobCleanUp.File = _file;

            _file.Stub(f => f.Delete(fileName)).Throw(new IOException());

            Assert.DoesNotThrow(() => jobCleanUp.DoCleanUp());
        }

        [Test]
        public void DeleteInfFile_DeletesInfFile()
        {
           _jobCleanUp.DoCleanUp();

            _file.AssertWasCalled(file => file.Delete(_infFile));
        }

        [Test]
        public void DeleteInfFile_IfDeleteThrowsExcepton_CatchesException()
        {
            _file.Stub(f => f.Delete(_infFile)).Throw(new IOException());
            Assert.DoesNotThrow(() => _jobCleanUp.DoCleanUp());
        }

        //TODO Delete folder if empty and not spool
    }
}
