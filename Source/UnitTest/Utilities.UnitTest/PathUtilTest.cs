using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class PathUtilTest
    {
        private PathUtil _pathUtil;
        private IPath _path;
        private IDirectory _directory;

        [SetUp]
        public void SetUp()
        {
            _path = Substitute.For<IPath>();
            _directory = Substitute.For<IDirectory>();
            _pathUtil = new PathUtil(_path, _directory);
        }

        [Test]
        public void EllipsisForPath_PathisNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _pathUtil.EllipsisForPath(null, _pathUtil.MAX_PATH));
        }

        [Test]
        public void EllipsisForPath_PathIsDirectory_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForPath("Directory\\", _pathUtil.MAX_PATH));
        }

        [Test]
        public void EllipsisForPath_MaxLengthShorterThan10_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForPath("Directory\\Filename.ext", 9));
        }

        [Test]
        public void EllipsisForPath_MaxLengthBiggerThanmaxpath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForPath("Directory\\Filename.ext", _pathUtil.MAX_PATH + 1));
        }

        [Test]
        public void EllipsisForPath_PathIsShorterThanMaxLength_ReturnsOriginalFilePath()
        {
            var filePath = "Directory\\Filename.ext";
            var length = filePath.Length;

            var filePathWithEllipsis = _pathUtil.EllipsisForPath(filePath, length + 1);

            Assert.AreEqual(filePath, filePathWithEllipsis);
        }

        [Test]
        public void EllipsisForPath_PathLengthEqualsMaxLength_ReturnsOriginalFilePath()
        {
            var filePath = "Directory\\Filename.ext";
            var length = filePath.Length;

            var filePathWithEllipsis = _pathUtil.EllipsisForPath(filePath, length);

            Assert.AreEqual(filePath, filePathWithEllipsis);
        }

        [Test]
        public void EllipsisForPath_TooLongPathButNotToBigToKeepDirectoryWithUsefullFilename_FileNameGetsEllipsis()
        {
            var directory = @"\\Very\Very\Long\Directory\Name\Without\Backslash";
            var extension = ".ext";
            var file = "Filename That Needs An Ellipsis Because It Is Too Long For Directory" + extension;
            var path = Path.Combine(directory, file);
            var maxLength = path.Length - file.Length + extension.Length + _pathUtil.ELLIPSIS.Length + 6; //enough space to shorten the filename to an useful length

            var pathWithEllipsis = _pathUtil.EllipsisForPath(path, maxLength);

            Assert.AreEqual(maxLength, pathWithEllipsis.Length, "Shortend path is too long");
            Assert.AreEqual(directory, Path.GetDirectoryName(pathWithEllipsis), "Directory was changed");
            Assert.AreEqual(Path.GetExtension(path), Path.GetExtension(pathWithEllipsis), "Extension was changed");
            StringAssert.Contains(_pathUtil.ELLIPSIS, Path.GetFileName(pathWithEllipsis), "Shortend file does not contain Ellipsis");
        }

        [Test]
        public void EllipsisForPath_TooLongPathToKeepDirectoryWithUsefullFilename_ThrowException()
        {
            var directory = @"\\Very\Very\Long\Directory\Name\Without\Backslash";
            var extension = ".ext";
            var file = "Filename That Needs An Ellipsis Because It Is Too Long For Directory" + extension;
            var path = Path.Combine(directory, file);
            var maxLength = path.Length - file.Length + extension.Length + 5; //Not enough space to shorten the filename to an useful length

            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForPath(path, maxLength));
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(".", false)]
        [TestCase(". ", false)]
        [TestCase(" . ", false)]
        [TestCase("folder. ", false)]
        [TestCase(@"folder.\ ", false)]
        [TestCase(@"fol.der\noextension", false)]
        [TestCase(@"folder.\noextension", false)]
        [TestCase(@".folder\noextension", false)]
        [TestCase("file.ext", true)]
        [TestCase(".ext", true)]
        [TestCase(@"folder\file.ext", true)]
        [TestCase(@"fol.der\file.ext", true)]
        [TestCase(@"folder\.ext", true)]
        public void HasExtension_RetunsExpectedResult(string path, bool expectedHasExtension)
        {
            Assert.AreEqual(expectedHasExtension, _pathUtil.HasExtension(path));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase(".", "")]
        [TestCase(". ", "")]
        [TestCase(" . ", "")]
        [TestCase("folder. ", "")]
        [TestCase(@"folder.\ ", "")]
        [TestCase(@"fol.der\noextension", "")]
        [TestCase(@"folder.\noextension", "")]
        [TestCase(@".folder\noextension", "")]
        [TestCase("file.ext", ".ext")]
        [TestCase(".ext", ".ext")]
        [TestCase(@"folder\file.ext", ".ext")]
        [TestCase(@"fol.der\file.ext", ".ext")]
        [TestCase(@"folder\.ext", ".ext")]
        [TestCase(@"folder\<>.ext", ".ext")]
        public void GetExtension_ReturnsExpectedExtension(string path, string expectedExtension)
        {
            Assert.AreEqual(expectedExtension, _pathUtil.GetExtension(path));
        }

        [TestCase(null, null, "")]
        [TestCase("path1", null, "path1")]
        [TestCase(null, "path2", "path2")]
        [TestCase("", "", "")]
        [TestCase("path1", "", "path1")]
        [TestCase("", "path2", "path2")]
        [TestCase("path1", "path2", @"path1\path2")]
        [TestCase(@"path1\", @"\path2", @"path1\path2")]
        [TestCase(@"path1\", @"path2", @"path1\path2")]
        [TestCase(@"path1", @"\path2", @"path1\path2")]
        [TestCase(@"\path1  \", @"\path2\", @"\path1\path2\")]
        [TestCase(@"\path1\\ \\", @"\\ \\  \path2\", @"\path1\path2\")]
        [TestCase(" path1 ", " path2 ", @"path1\path2")]
        [TestCase("<>", "<>", @"<>\<>")]
        public void Combine_ReturnsCombinedPath(string path1, string path2, string expectedCombine)
        {
            Assert.AreEqual(expectedCombine, _pathUtil.Combine(path1, path2));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("file", "file")]
        [TestCase(@"folder\file.ext", "file.ext")]
        [TestCase(@"folder\file", "file")]
        [TestCase(@"folder1\\file.ext", "file.ext")]
        [TestCase(@"folder1\\\file.ext", "file.ext")]
        [TestCase(@"folder\folder2\file.ext", "file.ext")]
        [TestCase(@"\file.ext", "file.ext")]
        [TestCase(@"<>\<>.ext", "<>.ext")]
        [TestCase("X:\\test.pdf", "test.pdf")]
        [TestCase("X:\\test\\", "")]
        public void GetFileName_ReturnsFileName(string path, string expectedFileName)
        {
            Assert.AreEqual(expectedFileName, _pathUtil.GetFileName(path));
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("file", "file")]
        [TestCase(@"folder\file.ext", "file")]
        [TestCase(@"folder\file", "file")]
        [TestCase(@"folder1\\file.ext", "file")]
        [TestCase(@"folder1\\\file.ext", "file")]
        [TestCase(@"folder\folder2\file.ext", "file")]
        [TestCase(@"\file.ext", "file")]
        [TestCase(@"<>\<>.ext", "<>")]
        public void GetFileNameWithoutExtension_ReturnsFileName(string path, string expectedFileName)
        {
            Assert.AreEqual(expectedFileName, _pathUtil.GetFileNameWithoutExtension(path));
        }

        [TestCase(null, "", "")]
        [TestCase("", null, "")]
        [TestCase("", "", "")]
        [TestCase("", ".ext", ".ext")]
        [TestCase("path", ".ext", "path.ext")]
        [TestCase("path.old", ".new", "path.new")]
        [TestCase(@"folder1\folder2\path.old", ".new", @"folder1\folder2\path.new")]
        [TestCase(@"folder1\folder2\<>.old", ".new", @"folder1\folder2\<>.new")]
        public void ChangeExtension_ReturnsPathWithChangedExtension(string path, string extension, string expectedPathWithChangedExtension)
        {
            Assert.AreEqual(expectedPathWithChangedExtension, _pathUtil.ChangeExtension(path, extension));
        }
    }
}
