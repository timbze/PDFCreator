using System;
using SystemInterface.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.IO;
using Rhino.Mocks;

namespace PDFCreator.Utilities.UnitTest.IO
{
    [TestFixture]
    public class UniqueFilenameTest
    {
        [Test]
        public void UniqueFileName_TestInitialising()
        {
            const string filename = @"C:\test.txt";
            var uniqueFilename = new UniqueFilename(filename);

            Assert.AreEqual(filename, uniqueFilename.OriginalFilename, "OriginalFilename is not setted file.");
            Assert.AreEqual(filename, uniqueFilename.LastUniqueFilename, "LastUniqueFilename is not setted file.");
        }

        [Test]
        public void UniqueFile_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UniqueFilename(null));
        }

        [Test]
        public void UniqueFile_GivenEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UniqueFilename("").CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenNonexistingFile_ReturnsSameFile()
        {
            const string filename = @"C:\test.txt";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(false);
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists("")).IgnoreArguments().Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);

            Assert.AreEqual(filename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFile_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test.txt";
            const string expectedFilename = @"C:\test_2.txt";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(true);
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false);
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists("")).IgnoreArguments().Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFile_LastUniqueFilenamePropertyIsUniquifiedFile()
        {
            const string filename = @"C:\test.txt";
            const string expectedFilename = @"C:\test_2.txt";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(true);
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false);
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists("")).IgnoreArguments().Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);
            uniqueFilename.CreateUniqueFileName();
            Assert.AreEqual(expectedFilename, uniqueFilename.LastUniqueFilename);
        }

        [Test]
        public void UniqueFile_GivenExistingFileWithoutExtension_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test";
            const string expectedFilename = @"C:\test_2";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(true);
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false);
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists("")).IgnoreArguments().Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFilenameOnSecondCall_AppendixGetsContinued()
        {
            const string filename = @"C:\test.jpg";
            const string firstUniquifiedFile = @"C:\test_2.jpg";
            const string expectedFilename = @"C:\test_3.jpg";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(true).Repeat.Once();
            fileWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(false).Repeat.Once();
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists("")).IgnoreArguments().Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);
            uniqueFilename.CreateUniqueFileName();
            fileWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(true);
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingDirectory_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test";
            const string expectedFilename = @"C:\test_2";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(false);
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false);
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists(filename)).Return(true);
            directoryWrap.Stub(x => x.Exists(expectedFilename)).Return(false);

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingDirectoryOnSecondCall_AppendixGetsContinued()
        {
            const string filename = @"C:\test";
            const string firstUniquifiedFile = @"C:\test_2";
            const string expectedFilename = @"C:\test_3";

            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(filename)).Return(false).Repeat.Once();
            fileWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(false).Repeat.Once();
            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists(filename)).Return(true).Repeat.Once();
            directoryWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(false).Repeat.Once();

            var uniqueFilename = new UniqueFilename(filename, directoryWrap, fileWrap);
            uniqueFilename.CreateUniqueFileName();

            fileWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(false).Repeat.Once();
            fileWrap.Stub(x => x.Exists(expectedFilename)).Return(false).Repeat.Once();
            directoryWrap.Stub(x => x.Exists(firstUniquifiedFile)).Return(true).Repeat.Once();
            directoryWrap.Stub(x => x.Exists(expectedFilename)).Return(false).Repeat.Once();
            
            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }
    }
}
