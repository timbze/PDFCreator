using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest.IO
{
    [TestFixture]
    internal class DirectoryHelperTest
    {
        private IDirectory _directoryMock;
        private List<string> _createdDirectoriesWith3Elements;

        private const string SomeFullPath = @"X:\secondparent\firstparent\directory";
        private const string PathWithTwoParts = @"X:\secondparent\firstparent";
        private const string PathWithOnePart = @"X:\secondparent";

        private void SetUp_DirectoryWithTwoParents()
        {
            _directoryMock = Substitute.For<IDirectory>();
            _createdDirectoriesWith3Elements = new[] { SomeFullPath, PathWithTwoParts, PathWithOnePart }.ToList();
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAllAlreadyExist_CreatedDirectoriesIsEmpty_ReturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists(Arg.Any<string>()).Returns(true);

            var directoryHelper = new DirectoryHelper(_directoryMock);
            var result = directoryHelper.CreateDirectory(PathWithTwoParts);

            Assert.IsEmpty(directoryHelper.CreatedDirectories);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAllDoNotExist_CreatedDirectoriesContainsAllDirectoriesStartingWithSmallest_ResultIsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists("").ReturnsForAnyArgs(false);

            var directoryHelper = new DirectoryHelper(_directoryMock);
            var result = directoryHelper.CreateDirectory(SomeFullPath);

            Assert.AreEqual(3, directoryHelper.CreatedDirectories.Count);
            Assert.AreEqual(PathWithOnePart, directoryHelper.CreatedDirectories[0], "Second parent is not the first created directory.");
            Assert.AreEqual(PathWithTwoParts, directoryHelper.CreatedDirectories[1], "First parent is not the second created directory.");
            Assert.AreEqual(SomeFullPath, directoryHelper.CreatedDirectories[2], "Full directory is not the last created directory.");
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAlreadyExist_CreatedDirectoriesContainsOnlyFullDirectory_ResultIsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists(PathWithOnePart).Returns(true);
            _directoryMock.Exists(PathWithTwoParts).Returns(true);
            _directoryMock.Exists(SomeFullPath).Returns(false);

            var directoryHelper = new DirectoryHelper(_directoryMock);
            var result = directoryHelper.CreateDirectory(SomeFullPath);

            Assert.AreEqual(1, directoryHelper.CreatedDirectories.Count);
            Assert.Contains(SomeFullPath, directoryHelper.CreatedDirectories);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWrapCreateDirectoryThrowsException_ReturnsFalse()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists("").ReturnsForAnyArgs(false);
            _directoryMock.CreateDirectory("").ThrowsForAnyArgs(new Exception());

            var directoryHelper = new DirectoryHelper(_directoryMock);
            var result = directoryHelper.CreateDirectory(SomeFullPath);

            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_AllCreatedDirectoriesAreEmpty_DirectoryDeleteGetsNeverCalled_ReturnsTrue()
        {
            var deleteCounter = 0;
            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = new DirectoryHelper(directoryMock);
            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_CreatedDirectoriesListIsEmpty_DirectoryWrapDeleteGetsNeverCalled_ReturnsTrue()
        {
            var deleteCounter = 0;
            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = new DirectoryHelper(directoryMock);
            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_DirectoryWrapDeleteDirectoryThrowsException_ReturnsFalse()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.GetFiles("").ReturnsForAnyArgs(new string[] { });
            _directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => { throw new Exception(); });
            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_FirstParentDirectoryContainsFile_OnlyFullDirectoryGetsDeleted_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            var directoryDeleted = false;
            var firstParentDeleted = false;
            var secondParentDeleted = false;

            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            _directoryMock.EnumerateFileSystemEntries(SomeFullPath).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(SomeFullPath)).Do(x => directoryDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(PathWithTwoParts).Returns(new[] { "some file.pdf" });
            _directoryMock.When(x => x.Delete(PathWithTwoParts)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(PathWithOnePart).Returns(new[] { "abc" });
            _directoryMock.When(x => x.Delete(PathWithOnePart)).Do(x => secondParentDeleted = true);

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsTrue(directoryDeleted);
            Assert.IsFalse(firstParentDeleted);
            Assert.IsFalse(secondParentDeleted);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_FullDirectoryContainsFile_NoDirectoriesGetDeleted_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();

            var deleteCounter = 0;
            _directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            _directoryMock.EnumerateFileSystemEntries(Arg.Any<string>()).Returns(new[] { "contains something entry" });
            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        //This test is needed, if create directory is called multiple times, while in the meantime some created directories were deleted.
        //They will be recreated and be added once more to the CreatedDirectoriesList.
        public void DeleteDirectory_FullDirectoryDoesNotExists_DoesNotTryToDeleteNotExistingDirectoryAndProceeds_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            var directoryDeleted = false;
            var firstParentDeleted = false;
            var secondParentDeleted = false;

            _directoryMock.Exists(SomeFullPath).Returns(false);
            _directoryMock.Exists(PathWithTwoParts).Returns(true);
            _directoryMock.Exists(PathWithOnePart).Returns(true);

            _directoryMock.EnumerateFileSystemEntries(PathWithTwoParts).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(PathWithTwoParts)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(PathWithOnePart).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(PathWithOnePart)).Do(x => secondParentDeleted = true);

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsFalse(directoryDeleted);
            Assert.IsTrue(firstParentDeleted);
            Assert.IsTrue(secondParentDeleted);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_SecondParentDirectoryContainsFile_FullDirectoryAndFirstParentGetDeleted_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            var directoryDeleted = false;
            var firstParentDeleted = false;
            var secondParentDeleted = false;

            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            _directoryMock.EnumerateFileSystemEntries(SomeFullPath).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(SomeFullPath)).Do(x => directoryDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(PathWithTwoParts).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(PathWithTwoParts)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(PathWithOnePart).Returns(new[] { "some file.pdf" });
            _directoryMock.When(x => x.Delete(PathWithOnePart)).Do(x => secondParentDeleted = true);

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsTrue(directoryDeleted);
            Assert.IsTrue(firstParentDeleted);
            Assert.IsFalse(secondParentDeleted);
            Assert.IsTrue(result);
        }

        [Test]
        public void GetDirectoryTree_DirectoryWithoutParents_ReturnListWithDirectoryAsSingleElement()
        {
            var myPath = "X:\\directoryFullName";
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            directoryInfoMock.FullName.Returns(myPath);

            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.GetParent("X:\\directoryFullName").ReturnsNull(); //-> No parents

            var directoryHelper = new DirectoryHelper(directoryMock);
            var directoryTree = directoryHelper.GetDirectoryTree(myPath);

            Assert.AreEqual(1, directoryTree.Count);
            Assert.Contains("X:\\directoryFullName", directoryTree);
        }

        [Test]
        public void GetDirectoryTree_DirectoryWithParents_ReturnListWithDirectoryAndParents()
        {
            SetUp_DirectoryWithTwoParents();

            var directoryHelper = new DirectoryHelper(_directoryMock);
            var directoryTree = directoryHelper.GetDirectoryTree(SomeFullPath);

            Assert.AreEqual(3, directoryTree.Count);
            Assert.Contains(SomeFullPath, directoryTree);
            Assert.Contains(PathWithTwoParts, directoryTree);
            Assert.Contains(PathWithOnePart, directoryTree);
        }

        [Test]
        public void Initialise_CreatedDirectoriesIsEmptyList()
        {
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            directoryInfoMock.FullName.Returns("directoryFullName");
            var directoryMock = Substitute.For<IDirectory>();

            var directoryHelper = new DirectoryHelper(directoryMock);

            Assert.IsEmpty(directoryHelper.CreatedDirectories);
        }

        [Test]
        public void CreateDirectory_WithTooLongPath_ReturnsFalse()
        {
            var directoryHelper = new DirectoryHelper(_directoryMock);

            var success = directoryHelper.CreateDirectory(@"X:\" + new string('a', 300));

            Assert.IsFalse(success);
        }

        [Test]
        public void CreateDirectory_InvalidCharacters_ReturnsFalse()
        {
            var directoryHelper = new DirectoryHelper(_directoryMock);

            var success = directoryHelper.CreateDirectory(@"X:\InvalidChars\<Token>");

            Assert.IsFalse(success);
        }
    }
}
