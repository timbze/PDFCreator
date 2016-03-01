using System.IO;
using SystemInterface.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Xps.OutputFileMover;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Xps.OutputFileMover
{
    [TestFixture]
    public class SingleFileMoverTest
    {
        private SingleFileMover _fileMover;
        private IFile _file;
        private IDirectory _directory;

        [SetUp]
        public void SetUp()
        {
            _file = MockRepository.GenerateStub<IFile>();
            _directory = MockRepository.GenerateStub<IDirectory>();

            _fileMover = new SingleFileMover(_file, _directory);
        }

        [Test]
        public void MoveSingleOutputFile_CopiesTempFileToFinalDestination()
        {
            _fileMover.MoveSingleOutputFile("test", "finalDir");
            _file.AssertWasCalled(f => f.Copy("test", "finalDir"));
        }

        [Test]
        public void MoveSingleOutputFile_DeletesTempFile()
        {
            _fileMover.MoveSingleOutputFile("test", "finalDir");

            _file.AssertWasCalled(f => f.Delete("test"));
        }

        [Test]
        public void MoveSingleOutputFile_ReturnsOutputFile()
        {
            _fileMover.UniqueFileNameEnabled = true;

            var tempFile = "theTempFile";
            var targetFile = "theTarget";

            _file.Stub(f => f.Exists(targetFile)).Return(false);

            var outputFile = _fileMover.MoveSingleOutputFile(tempFile, targetFile);

            Assert.AreEqual(targetFile, outputFile);
        }

        [Test]
        public void MoveSingleOutputFile_IfUniqueFileNameIsDisabled_CopyToExistingFile()
        {
            _fileMover.UniqueFileNameEnabled = false;

            var tempFile = "theTempFile";
            var targetFile = "thisIsTheTarget";
 
            _file.Stub(f => f.Exists(targetFile)).Return(true);

            _fileMover.MoveSingleOutputFile(tempFile, targetFile);

            _file.AssertWasCalled(f => f.Copy(tempFile, targetFile));
        }

        [Test]
        public void MoveSingleOutputFile_IfUniqueFileNameIsEnabled_EnsureUniqueFileName()
        {
            _fileMover.UniqueFileNameEnabled = true;

            var tempFile = "theTempFile";
            var targetFile = "thisIsTheTarget";
            var excpectedTarget = targetFile + "_2";

            _file.Stub(f => f.Exists(targetFile)).Return(true);
            _file.Stub(f => f.Exists(excpectedTarget)).Return(false);

            _fileMover.MoveSingleOutputFile(tempFile, targetFile);

            _file.AssertWasCalled(f => f.Copy(tempFile, excpectedTarget));
        }

        [Test]
        public void MoveSingleOutputFile_IfCopyIsNotSuccessfulOnSecondTry_ThrowException()
        {
            var tempFile = "theTempFile";
            var targetFile = "thisIsTheTarget";

            _file.Stub(f => f.Exists(targetFile)).Return(true);
            _file.Stub(f => f.Copy(tempFile, targetFile)).IgnoreArguments().Throw(new IOException());

            Assert.Throws<IOException>(() => _fileMover.MoveSingleOutputFile(tempFile, targetFile));

        }

        [Test]
        public void MoveSingleOutputFile_IfCopyIsNotSuccessfulRetryCopyWithUniqueFilename()
        {
            _fileMover.UniqueFileNameEnabled = false;
            
            var tempFile = "theTempFile";
            var targetFile = "thisIsTheTarget";
            var excpectedTarget = targetFile + "_2";

            _file.Stub(f => f.Exists(targetFile)).Return(true);
            _file.Stub(f => f.Exists(excpectedTarget)).Return(false);
            _file.Stub(f => f.Copy(tempFile, targetFile)).Throw(new IOException()).Repeat.Once();

            _fileMover.MoveSingleOutputFile(tempFile, targetFile);

            _file.AssertWasCalled(f => f.Copy(tempFile, excpectedTarget));
        }

        [Test]
        public void MoveSingleOutputFile_IfDeleteIsNotSuccessful_DoesNotThrowException()
        {
            _file.Stub(f => f.Delete("test")).Throw(new IOException());
            Assert.DoesNotThrow(() => _fileMover.MoveSingleOutputFile("test", "finalDir"));
        }

        [Test]
        public void MoveSingleOutputFile_IfDirectoryDoesNotExist_CreatesDirectory()
        {
            var tempFile = "theTempFile";
            var outputFolder = "someFolder";
            var targetFile = Path.Combine(outputFolder, "theTarget");
            _directory.Stub(d => d.Exists(outputFolder)).Return(false);

            _fileMover.MoveSingleOutputFile(tempFile, targetFile);

            _directory.AssertWasCalled(d => d.CreateDirectory(outputFolder));
        }        
    }
}
