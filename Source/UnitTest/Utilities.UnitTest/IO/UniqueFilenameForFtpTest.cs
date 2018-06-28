using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest.IO
{
    [TestFixture]
    internal class UniqueFilenameForFtpTest
    {
        [SetUp]
        public void SetUp()
        {
            _ftpConnectionWrap = Substitute.For<IFtpConnection>();
            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.MAX_PATH.Returns(259);
            _uniqueFilenameForFtp = new UniqueFilenameForFtp(Filename, _ftpConnectionWrap, _pathUtil);
        }

        private const string Filename = @"test.txt";
        private UniqueFilenameForFtp _uniqueFilenameForFtp;
        private IFtpConnection _ftpConnectionWrap;
        private IPathUtil _pathUtil;

        [Test]
        public void UniqueFile_GivenExistingFile_ReturnsUniquifiedFile()
        {
            const string expectedFilename = @"test_2.txt";
            _ftpConnectionWrap.FileExists(Filename).Returns(true);
            _ftpConnectionWrap.FileExists(expectedFilename).Returns(false);
            _pathUtil.EllipsisForTooLongPath(expectedFilename).Returns(expectedFilename);

            Assert.AreEqual(expectedFilename, _uniqueFilenameForFtp.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFilenameOnSecondCall_AppendixGetsContinued()
        {
            const string firstUniquifiedFile = @"test_2.txt";
            const string expectedFilename = @"test_3.txt";

            _ftpConnectionWrap.FileExists(Filename).Returns(true);
            _ftpConnectionWrap.FileExists(firstUniquifiedFile).Returns(true);
            _ftpConnectionWrap.FileExists(expectedFilename).Returns(false);
            _pathUtil.EllipsisForTooLongPath(firstUniquifiedFile).Returns(firstUniquifiedFile);
            _pathUtil.EllipsisForTooLongPath(expectedFilename).Returns(expectedFilename);

            Assert.AreEqual(expectedFilename, _uniqueFilenameForFtp.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenNonexistingFile_ReturnsSameFile()
        {
            _ftpConnectionWrap.FileExists(Filename).Returns(false);

            Assert.AreEqual(Filename, _uniqueFilenameForFtp.CreateUniqueFileName());
        }
    }
}
