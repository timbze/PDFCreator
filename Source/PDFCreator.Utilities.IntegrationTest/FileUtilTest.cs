using System;
using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;
using PDFCreator.TestUtilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    class FileUtilTest
    {
        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [TestCase(@"C:\Test.txt")]
        [TestCase(@"A:\Test.txt")]
        [TestCase(@"Z:\Test.txt")]
        [TestCase(@"X:\Test\Folder\With\Many\Sub\Folders\test.txt")]
        [TestCase(@"\\TestServer\SomeFolder\Test.txt")]
        public void TestValidRootedPath(string validPath)
        {
            Assert.IsTrue(FileUtil.Instance.IsValidRootedPath(validPath), "Expected '" + validPath + "' to be a valid path");
        }

        [TestCase("text.txt")]
        [TestCase(@":@!|")]
        [TestCase(@"\Test\test.txt")]
        [TestCase(@":\test.txt")]
        [TestCase(@"_:\Test.txt")]
        [TestCase("")]
        [TestCase("a")]
        [TestCase("C:MyDir")]
        [TestCase(@"C:\Mydir:")]
        public void TestInvalidRootedPath(string invalidPath)
        {
            Assert.IsFalse(FileUtil.Instance.IsValidRootedPath(invalidPath), "Expected '" + invalidPath + "' to be an invalid path");
        }

        [Test]
        public void MakeValidFileName_GivenValidFilename_ReturnsSameString()
        {
            const string filename = @"Test ! File.txt";
            Assert.AreEqual(filename, FileUtil.Instance.MakeValidFileName(filename));
        }

        [Test]
        public void MakeValidFileName_GivenInvalidFilename_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"File_Name", FileUtil.Instance.MakeValidFileName(@"File:Name"));
        }

        [Test]
        public void MakeValidFolderName_GivenValidFolder_ReturnsSameString()
        {
            const string filename = @"C:\Some _ !Folder,";
            Assert.AreEqual(filename, FileUtil.Instance.MakeValidFolderName(filename));
        }

        [Test]
        public void MakeValidFolderName_GivenInvalidFolder_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some _ Folder", FileUtil.Instance.MakeValidFolderName(@"C:\Some | Folder"));
        }

        [Test]
        public void MakeValidFolderName_GivenMisplacedColon_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some_Folder", FileUtil.Instance.MakeValidFolderName(@"C:\Some:Folder"));
        }

        [Test]
        public void TestIsValidFilename()
        {
            string[] validPaths = { @"C:\Test.txt", @"X:\Test\Folder\With\Many\Sub\Folders\test.txt", @"C:\Test,abc.txt" };
            string[] invalidPaths = { @"C:\Test<.txt", @"C:\Test>.txt", @"C:\Test?.txt", @"C:\Test*.txt", @"C:\Test|.txt", @"C:\Test"".txt" };

            foreach (var p in validPaths)
                Assert.IsTrue(FileUtil.Instance.IsValidFilename(p), "Expected '" + p + "' to be a valid path");

            foreach (var p in invalidPaths)
                Assert.IsFalse(FileUtil.Instance.IsValidFilename(p), "Expected '" + p + "' to be an invalid path");
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasPrintVerb()
        {
            Assert.IsTrue(FileUtil.Instance.FileAssocHasPrint("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasPrintToVerb()
        {
            Assert.IsTrue(FileUtil.Instance.FileAssocHasPrintTo("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasOpenVerb()
        {
            Assert.IsTrue(FileUtil.Instance.FileAssocHasOpen("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenEmpty_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtil.Instance.FileAssocHasPrintTo(""));
        }

        [Test]
        public void FileAssocHasPrint_GivenDot_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => FileUtil.Instance.FileAssocHasPrintTo("."));
        }

        [Test]
        public void FileAssocHasPrint_GivenDoubleDot_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => FileUtil.Instance.FileAssocHasPrintTo(".."));
        }

        [Test]
        public void FileAssocHasPrint_GivenUnknownExtension_ReturnsFalse()
        {
            Assert.IsFalse(FileUtil.Instance.FileAssocHasPrintTo(".unkownFileExtension"));
        }

        [Test]
        public void CommandLineToArgs_GivenSimpleCommandLine_ReturnsGoodArray()
        {
            const string commandLine = "/Test /Quote=\"This is a Test\"";
            var expected = new[] { "/Test", "/Quote=This is a Test" };
            Assert.AreEqual(expected, FileUtil.Instance.CommandLineToArgs(commandLine));
        }

        [Test]
        public void GetLongDirectoryName_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtil.Instance.GetLongDirectoryName(null));
        }

        [Test]
        public void GetLongDirectoryName_GivenEmptyPath_ReturnsNull()
        {
            Assert.IsNull(FileUtil.Instance.GetLongDirectoryName(""));
        }

        [Test]
        public void GetLongDirectoryName_GivenShortPath_ReturnsSamePath()
        {
            string folder = @"C:\folder";
            string file = folder + "\\test.txt";

            Assert.AreEqual(folder, FileUtil.Instance.GetLongDirectoryName(file));
        }

        [Test]
        public void GetLongDirectoryName_GivenDriveRootWithFile_ReturnsDriveRoot()
        {
            string folder = @"C:\";
            string file = folder + "\\test.txt";

            Assert.AreEqual(folder, FileUtil.Instance.GetLongDirectoryName(file));
        }

        [Test]
        public void CalculateMd5_WithNullFile_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtil.Instance.CalculateMd5(null));
        }

        [Test]
        public void CalculateMd5_WithEmptyFile_CalculatesCorrectHash()
        {
            string tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "");

            Assert.AreEqual("d41d8cd98f00b204e9800998ecf8427e", FileUtil.Instance.CalculateMd5(tmpFile));
        }

        [Test]
        public void CalculateMd5_WithTestString_CalculatesCorrectHash()
        {
            string tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.AreEqual("098f6bcd4621d373cade4e832627b4f6", FileUtil.Instance.CalculateMd5(tmpFile));
        }

        [Test]
        public void VerifyMd5_WithTestStringAndCorrectMd5_ReturnsTrue()
        {
            string tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.IsTrue(FileUtil.Instance.VerifyMd5(tmpFile, "098f6bcd4621d373cade4e832627b4f6"));
        }

        [Test]
        public void VerifyMd5_WithTestStringAndIncorrectMd5_ReturnsFalse()
        {
            string tmpFile = TempFileHelper.CreateTempFile("MD5_Test", "file.txt");
            File.WriteAllText(tmpFile, "test");

            Assert.IsFalse(FileUtil.Instance.VerifyMd5(tmpFile, "d41d8cd98f00b204e9800998ecf8427e"));
        }

        [Test]
        public void CheckWritability_WithWritableFolder_ReturnsTrue()
        {
            string tmpFolder = TempFileHelper.CreateTempFolder("CheckWritability");
            Assert.IsTrue(FileUtil.Instance.CheckWritability(tmpFolder));
        }
    }
}
