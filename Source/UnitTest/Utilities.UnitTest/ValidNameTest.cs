using NUnit.Framework;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class ValidNameTest
    {
        [SetUp]
        public void Setup()
        {
            _validName = new ValidName();
        }

        private ValidName _validName;

        [Test]
        public void MakeValidFileName_GivenInvalidFilename_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"File_Name", _validName.MakeValidFileName(@"File:Name"));
        }

        [Test]
        public void MakeValidFileName_GivenValidFilename_ReturnsSameString()
        {
            const string filename = @"Test ! File.txt";
            Assert.AreEqual(filename, _validName.MakeValidFileName(filename));
        }

        [Test]
        public void MakeValidFolderName_GivenInvalidFolder_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some _ Folder", _validName.MakeValidFolderName(@"C:\Some | Folder"));
        }

        [Test]
        public void MakeValidFolderName_GivenMisplacedColon_ReturnsSanitizedString()
        {
            Assert.AreEqual(@"C:\Some_Folder", _validName.MakeValidFolderName(@"C:\Some:Folder"));
        }

        [Test]
        public void MakeValidFolderName_GivenValidFolder_ReturnsSameString()
        {
            const string filename = @"C:\Some _ !Folder,";
            Assert.AreEqual(filename, _validName.MakeValidFolderName(filename));
        }

        [Test]
        public void TestIsValidFilename()
        {
            string[] validPaths = {@"C:\Test.txt", @"X:\Test\Folder\With\Many\Sub\Folders\test.txt", @"C:\Test,abc.txt"};
            string[] invalidPaths = {@"C:\Test<.txt", @"C:\Test>.txt", @"C:\Test?.txt", @"C:\Test*.txt", @"C:\Test|.txt", @"C:\Test"".txt"};

            foreach (var p in validPaths)
                Assert.IsTrue(_validName.IsValidFilename(p), "Expected '" + p + "' to be a valid path");

            foreach (var p in invalidPaths)
                Assert.IsFalse(_validName.IsValidFilename(p), "Expected '" + p + "' to be an invalid path");
        }
    }
}
