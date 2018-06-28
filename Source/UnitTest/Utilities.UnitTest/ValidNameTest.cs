using NUnit.Framework;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class ValidNameTest
    {
        [Test]
        public void MakeValidFileName_GivenInvalidFilename_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"File_Name", ValidName.MakeValidFileName(@"File:Name"));
        }

        [Test]
        public void MakeValidFileName_GivenValidFilename_ReturnsSameString()
        {
            const string filename = @"Test ! File.txt";
            Assert.AreEqual(filename, ValidName.MakeValidFileName(filename));
        }

        [Test]
        public void MakeValidFolderName_GivenInvalidFolder_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some _ Folder", ValidName.MakeValidFolderName(@"C:\Some | Folder"));
        }

        [Test]
        public void MakeValidFolderName_GivenMisplacedColon_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some_Folder", ValidName.MakeValidFolderName(@"C:\Some:Folder"));
        }

        [Test]
        public void MakeValidFolderName_GivenValidFolder_ReturnsSameString()
        {
            const string filename = @"C:\Some _ !Folder,";
            Assert.AreEqual(filename, ValidName.MakeValidFolderName(filename));
        }

        [Test]
        public void TestIsValidFilename()
        {
            string[] validPaths = { @"C:\Test.txt", @"X:\Test\Folder\With\Many\Sub\Folders\test.txt", @"C:\Test,abc.txt" };
            string[] invalidPaths = { @"C:\Test<.txt", @"C:\Test>.txt", @"C:\Test?.txt", @"C:\Test*.txt", @"C:\Test|.txt", @"C:\Test"".txt" };

            foreach (var p in validPaths)
                Assert.IsTrue(ValidName.IsValidPath(p), "Expected '" + p + "' to be a valid path");

            foreach (var p in invalidPaths)
                Assert.IsFalse(ValidName.IsValidPath(p), "Expected '" + p + "' to be an invalid path");
        }
    }
}
