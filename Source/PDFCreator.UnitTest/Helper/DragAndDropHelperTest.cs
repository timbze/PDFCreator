using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Helper;
using Rhino.Mocks;
using Arg = NSubstitute.Arg;

namespace PDFCreator.UnitTest.Helper
{

    [TestFixture]
    class DragAndDropHelperTest
    {
        [Test]
        public void IsDragAndDrop_ArgsIsNull_ReturnsFalse()
        {
            string[] args = null;
            Assert.IsFalse(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void IsDragAndDrop_ArgsIsEmptyList_ReturnsFalse()
        {
            var args = new string[0];
            Assert.IsFalse(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void IsDragAndDrop_ArgsContainsStringWithSlash_ReturnsFalse()
        {
            string[] args = { "/slashParameter" };
            Assert.IsFalse(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void IsDragAndDrop_ArgsContainsStringsWithSlash_ReturnsFalse()
        {
            string[] args = { "someParameter1", "/slashParameter", "someParameter2" };
            Assert.IsFalse(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void IsDragAndDrop_ArgsContainsStringWithoutSlash_ReturnsTrue()
        {
            string[] args = { "someParameter" };
            Assert.IsTrue(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void IsDragAndDrop_ArgsContainsStringsWithoutSlash_ReturnsTrue()
        {
            string[] args = { "someParameter1", "someParameter2" , "someParameter3" };
            Assert.IsTrue(DragAndDropHelper.IsDragAndDrop(args));
        }

        [Test]
        public void RemoveInvalidFiles_InsertExistingFiles_ReturnsAllFiles()
        {
            var files = new []{"file1", "file2", "file3"};
            var fileStub = Substitute.For<IFile>();
            fileStub.Exists(Arg.Any<string>()).Returns(true);

            Assert.AreEqual(files, DragAndDropHelper.RemoveInvalidFiles(files, fileStub));
        }

        [Test]
        public void RemoveInvalidFiles_InsertNotExistingFiles_ReturnsEmptyList()
        {
            var files = new[] { "file1", "file2", "file3" };
            var fileStub = Substitute.For<IFile>();
            fileStub.Exists(Arg.Any<string>()).Returns(false);

            Assert.IsEmpty(DragAndDropHelper.RemoveInvalidFiles(files, fileStub));
        }

        [Test]
        public void RemoveInvalidFiles_InsertListWithNotExistingFile_ReturnsListWithoutNotExistingFile()
        {
            var files = new[] { "existing file1", "not exsiting file", "existing file2" };
            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("existing file1").Returns(true);
            fileStub.Exists("not exsiting file").Returns(false);
            fileStub.Exists("existing file2").Returns(true);

            var validFiles = DragAndDropHelper.RemoveInvalidFiles(files, fileStub).ToList();

            Assert.Contains("existing file1", validFiles);
            Assert.IsFalse(validFiles.Contains("not exsiting file"));
            Assert.Contains("existing file2", validFiles);
        }


    }
}
