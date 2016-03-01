using System;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    class FileUtilEllipsisTest
    {
        [Test]
        public void Ellipsis_GivenNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtil.Instance.EllipsisForTooLongPath(null));
        }

        [Test]
        public void Ellipsis_GivenEmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual("", FileUtil.Instance.EllipsisForTooLongPath(""));
        }

        [Test]
        public void Ellipsis_GivenShortFile_ReturnsSameString()
        {
            const string file = @"C:\Test\abc.txt";
            Assert.AreEqual(file, FileUtil.Instance.EllipsisForTooLongPath(file));
        }

        [Test]
        public void Ellipsis_Given300CharFile_ReturnsShortenedString()
        {
            string file = @"C:\folder\" + new string('a', 293) + ".txt";

            string shortFile = FileUtil.Instance.EllipsisForTooLongPath(file);

            Assert.AreNotEqual(file, shortFile);
            Assert.IsTrue(shortFile.Contains("(...)"));
            Assert.IsTrue(shortFile.Length <= FileUtil.MAX_PATH);
        }

        [Test]
        public void Ellipsis_Given300CharFileWithoutExtension_ReturnsShortenedString()
        {
            string file = @"C:\" + new string('a', 297);

            string shortFile = FileUtil.Instance.EllipsisForTooLongPath(file);

            Assert.IsTrue(shortFile.Contains("(...)"), "File does not contain ellipsis");
            Assert.IsTrue(shortFile.Length <= FileUtil.MAX_PATH, "Shortened file still is too long");
        }

        [Test]
        public void Ellipsis_Given300CharFolderWithFile_ThrowsException()
        {
            string folder = @"C:\" + new string('a', 300) + "\\";
            string file = folder + "test.txt";

            Assert.Throws<ArgumentException>(() => FileUtil.Instance.EllipsisForTooLongPath(file));
        }

        [Test]
        public void Ellipsis_FolderWithoutFile_ThrowsException()
        {
            const string folder = @"C:\folder\";

            Assert.Throws<ArgumentException>(() => FileUtil.Instance.EllipsisForTooLongPath(folder));
        }

        [Test]
        public void EllipsisWithLength_Given300CharFile_ReturnsShortenedString()
        {
            string file = @"C:\" + new string('a', 293) + ".txt";

            string shortFile = FileUtil.Instance.EllipsisForPath(file, 200);

            Assert.AreNotEqual(file, shortFile);
            Assert.IsTrue(shortFile.Contains("(...)"));
            Assert.IsTrue(shortFile.Length <= 200);
        }

        [Test]
        public void EllipsisWithLength_GivenLength9_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => FileUtil.Instance.EllipsisForPath(@"C:\test.txt", 9));
        }

        [Test]
        public void EllipsisWithLength_GivenLength261_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => FileUtil.Instance.EllipsisForPath(@"C:\test.txt", FileUtil.MAX_PATH + 1));
        }
    }
}
