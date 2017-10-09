using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;
using System;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    internal class FileAssocTest
    {
        [SetUp]
        public void SetUp()
        {
            _fileAssoc = new FileAssoc();
        }

        private IFileAssoc _fileAssoc;

        public void IllegalDotAssocCall()
        {
            _fileAssoc.HasPrint(".illegal.fileextension");
        }

        private void IllegalShortAssocCall()
        {
            _fileAssoc.HasPrint(".");
        }

        private void NullAssocCall()
        {
            _fileAssoc.HasPrint(null);
        }

        private void EmptyAssocCall()
        {
            _fileAssoc.HasPrint("");
        }

        [Test]
        public void FileAssocHasPrint_GivenDot_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _fileAssoc.HasPrintTo("."));
        }

        [Test]
        public void FileAssocHasPrint_GivenDoubleDot_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _fileAssoc.HasPrintTo(".."));
        }

        [Test]
        public void FileAssocHasPrint_GivenEmpty_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _fileAssoc.HasPrintTo(""));
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasOpenVerb()
        {
            Assert.IsTrue(_fileAssoc.HasOpen("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasPrintToVerb()
        {
            Assert.IsTrue(_fileAssoc.HasPrintTo("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenTxt_HasPrintVerb()
        {
            Assert.IsTrue(_fileAssoc.HasPrint("txt"));
        }

        [Test]
        public void FileAssocHasPrint_GivenUnknownExtension_ReturnsFalse()
        {
            Assert.IsFalse(_fileAssoc.HasPrintTo(".unkownFileExtension"));
        }

        [Test]
        public void TestDefaultAssoc()
        {
            Assert.IsTrue(_fileAssoc.HasPrintTo(".txt"), "PrintTo association for .txt files not detected!");
            Assert.IsTrue(_fileAssoc.HasPrint(".txt"), "Print association for .txt files not detected!");
            Assert.IsTrue(_fileAssoc.HasPrintTo("txt"), "PrintTo association for .txt files not detected!");
            Assert.IsFalse(_fileAssoc.HasPrintTo(".invalidfileextension"), "PrintTo association for .invalidfileextension files detected, but should not exist!");
            Assert.IsFalse(_fileAssoc.HasPrint(".invalidfileextension"), "Print association for .invalidfileextension files detected, but should not exist!");
        }

        [Test]
        public void TestExceptions()
        {
            Assert.Throws<ArgumentException>(IllegalDotAssocCall);
            Assert.Throws<ArgumentException>(IllegalShortAssocCall);
            Assert.Throws<ArgumentNullException>(NullAssocCall);
            Assert.Throws<ArgumentNullException>(EmptyAssocCall);
        }
    }
}
