using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Shared.Helper;

namespace PDFCreator.Shared.Test.Helper
{
    [TestFixture]
    class DirectoryHelperTest
    {
        private IDirectoryInfo _fullDirectoryInfoMock;
        private IDirectoryInfo _firstParentDirectoryInfoMock;
        private IDirectoryInfo _secondParentDirectoryInfoMock;
        private IDirectory _directoryMock;
        private List<string> _createdDirectoriesWith3Elements;

        private void SetUp_DirectoryWithTwoParents()
        {
            _fullDirectoryInfoMock = Substitute.For<IDirectoryInfo>();
            _fullDirectoryInfoMock.FullName.Returns(@"\secondparent\firstparent\directory");

            _firstParentDirectoryInfoMock = Substitute.For<IDirectoryInfo>();
            _firstParentDirectoryInfoMock.FullName.Returns(@"\secondparent\firstparent");

            _secondParentDirectoryInfoMock = Substitute.For<IDirectoryInfo>();
            _secondParentDirectoryInfoMock.FullName.Returns(@"\secondparent");

            _directoryMock = Substitute.For<IDirectory>();
            _directoryMock.GetParent(_fullDirectoryInfoMock.FullName).Returns(_firstParentDirectoryInfoMock);
            _directoryMock.GetParent(_firstParentDirectoryInfoMock.FullName).Returns(_secondParentDirectoryInfoMock);
            _directoryMock.GetParent(_secondParentDirectoryInfoMock.FullName).ReturnsNull(); // -> No further parents  

            _createdDirectoriesWith3Elements = new[]{_fullDirectoryInfoMock.FullName, _firstParentDirectoryInfoMock.FullName, _secondParentDirectoryInfoMock.FullName}.ToList();
        }

        [Test]
        public void Initialise_DirectoryIsFullPath()
        {
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            directoryInfoMock.FullName.Returns("directoryFullName");
            var directoryMock = Substitute.For<IDirectory>();

            var directoryHelper = new DirectoryHelper(directoryInfoMock, directoryMock);

            Assert.AreEqual(directoryHelper.Directory, "directoryFullName");
        }

        [Test]
        public void Initialise_CreatedDirectoriesIsEmptyList()
        {
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            directoryInfoMock.FullName.Returns("directoryFullName");
            var directoryMock = Substitute.For<IDirectory>();

            var directoryHelper = new DirectoryHelper(directoryInfoMock, directoryMock);

            Assert.IsEmpty(directoryHelper.CreatedDirectories);
        }

        [Test]
        public void GetDirectoryTree_DirectoryWithoutParents_ReturnListWithDirectoryAsSingleElement()
        {            
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            directoryInfoMock.FullName.Returns("directoryFullName");

            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.GetParent("directoryFullName").ReturnsNull(); //-> No parents

            var directoryHelper = new DirectoryHelper(directoryInfoMock, directoryMock);
            var directoryTree = directoryHelper.GetDirectoryTree();

            Assert.AreEqual(1, directoryTree.Count);
            Assert.Contains("directoryFullName", directoryTree);
        }

        [Test]
        public void GetDirectoryTree_DirectoryWithParents_ReturnListWithDirectoryAndParents()
        {
            SetUp_DirectoryWithTwoParents();

            var directoryHelper = new DirectoryHelper(_fullDirectoryInfoMock, _directoryMock);
            var directoryTree = directoryHelper.GetDirectoryTree();

            Assert.AreEqual(3, directoryTree.Count);
            Assert.Contains(_fullDirectoryInfoMock.FullName, directoryTree);
            Assert.Contains(_firstParentDirectoryInfoMock.FullName, directoryTree);
            Assert.Contains(_secondParentDirectoryInfoMock.FullName, directoryTree);
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAllAlreadyExist_CreatedDirectoriesIsEmpty_ReturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists("").ReturnsForAnyArgs(true);

            var directoryHelper = new DirectoryHelper(_fullDirectoryInfoMock, _directoryMock);
            var result = directoryHelper.CreateDirectory();

            Assert.IsEmpty(directoryHelper.CreatedDirectories);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAllDoNotExist_CreatedDirectoriesContainsAllDirectoriesStartingWithSmallest_ResultIsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists("").ReturnsForAnyArgs(false);

            var directoryHelper = new DirectoryHelper(_fullDirectoryInfoMock, _directoryMock);
            var result = directoryHelper.CreateDirectory();

            Assert.AreEqual(3, directoryHelper.CreatedDirectories.Count);
            Assert.AreEqual(_secondParentDirectoryInfoMock.FullName, directoryHelper.CreatedDirectories[0], "Second parent is not the first created directory.");
            Assert.AreEqual(_firstParentDirectoryInfoMock.FullName, directoryHelper.CreatedDirectories[1], "First parent is not the second created directory.");
            Assert.AreEqual(_fullDirectoryInfoMock.FullName, directoryHelper.CreatedDirectories[2], "Full directory is not the last created directory.");
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWithTwoParentsThatAlreadyExist_CreatedDirectoriesContainsOnlyFullDirectory_ResultIsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists(_secondParentDirectoryInfoMock.FullName).Returns(true);
            _directoryMock.Exists(_firstParentDirectoryInfoMock.FullName).Returns(true);
            _directoryMock.Exists(_fullDirectoryInfoMock.FullName).Returns(false);

            var directoryHelper = new DirectoryHelper(_fullDirectoryInfoMock, _directoryMock);
            var result = directoryHelper.CreateDirectory();

            Assert.AreEqual(1, directoryHelper.CreatedDirectories.Count);
            Assert.Contains(_fullDirectoryInfoMock.FullName, directoryHelper.CreatedDirectories);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateDirectory_DirectoryWrapCreateDirectoryThrowsException_ReturnsFalse()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.Exists("").ReturnsForAnyArgs(false);
            _directoryMock.CreateDirectory("").ThrowsForAnyArgs(new Exception());

            var directoryHelper = new DirectoryHelper(_fullDirectoryInfoMock, _directoryMock);
            var result = directoryHelper.CreateDirectory();

            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDirectory_CreatedDirectoriesListIsEmpty_DirectoryWrapDeleteGetsNeverCalled_ReturnsTrue()
        {
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            var deleteCounter = 0;
            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist
            
            var directoryHelper = new DirectoryHelper(directoryInfoMock, directoryMock);
            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_AllCreatedDirectoriesAreEmpty_DirectoryDeleteGetsNeverCalled_ReturnsTrue()
        {
            var directoryInfoMock = Substitute.For<IDirectoryInfo>();
            var deleteCounter = 0;
            var directoryMock = Substitute.For<IDirectory>();
            directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = new DirectoryHelper(directoryInfoMock, directoryMock);
            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_FullDirectoryContainsFile_NoDirectoriesGetDeleted_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            
            var deleteCounter = 0;
            _directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => deleteCounter++);
            _directoryMock.EnumerateFileSystemEntries(_fullDirectoryInfoMock.FullName).Returns(new[]{"contains something entry"});
            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_fullDirectoryInfoMock, _directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.AreEqual(0, deleteCounter);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_FirstParentDirectoryContainsFile_OnlyFullDirectoryGetsDeleted_ResturnsTrue()
        {
            SetUp_DirectoryWithTwoParents();
            var directoryDeleted = false;
            var firstParentDeleted = false;
            var secondParentDeleted = false;

            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            _directoryMock.EnumerateFileSystemEntries(_fullDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_fullDirectoryInfoMock.FullName)).Do(x => directoryDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(_firstParentDirectoryInfoMock.FullName).Returns(new[] { "some file.pdf" });
            _directoryMock.When(x => x.Delete(_firstParentDirectoryInfoMock.FullName)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(_secondParentDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_secondParentDirectoryInfoMock.FullName)).Do(x => secondParentDeleted = true);
            
            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_fullDirectoryInfoMock, _directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();
            
            Assert.IsTrue(directoryDeleted);
            Assert.IsFalse(firstParentDeleted);
            Assert.IsFalse(secondParentDeleted);
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

            _directoryMock.EnumerateFileSystemEntries(_fullDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_fullDirectoryInfoMock.FullName)).Do(x => directoryDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(_firstParentDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_firstParentDirectoryInfoMock.FullName)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(_secondParentDirectoryInfoMock.FullName).Returns(new[] { "some file.pdf" });
            _directoryMock.When(x => x.Delete(_secondParentDirectoryInfoMock.FullName)).Do(x => secondParentDeleted = true);

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_fullDirectoryInfoMock, _directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsTrue(directoryDeleted);
            Assert.IsTrue(firstParentDeleted);
            Assert.IsFalse(secondParentDeleted);
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

            _directoryMock.Exists(_fullDirectoryInfoMock.FullName).Returns(false);
            _directoryMock.Exists(_firstParentDirectoryInfoMock.FullName).Returns(true);
            _directoryMock.Exists(_secondParentDirectoryInfoMock.FullName).Returns(true);

            _directoryMock.EnumerateFileSystemEntries(_firstParentDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_firstParentDirectoryInfoMock.FullName)).Do(x => firstParentDeleted = true);
            _directoryMock.EnumerateFileSystemEntries(_secondParentDirectoryInfoMock.FullName).Returns(new string[] { });
            _directoryMock.When(x => x.Delete(_secondParentDirectoryInfoMock.FullName)).Do(x => secondParentDeleted = true);

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_fullDirectoryInfoMock, _directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsFalse(directoryDeleted);
            Assert.IsTrue(firstParentDeleted);
            Assert.IsTrue(secondParentDeleted);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDirectory_DirectoryWrapDeleteDirectoryThrowsException_ReturnsFalse()
        {
            SetUp_DirectoryWithTwoParents();
            _directoryMock.GetFiles("").ReturnsForAnyArgs(new string[] {});
            _directoryMock.WhenForAnyArgs(x => x.Delete("")).Do(x => { throw new Exception(); });
            _directoryMock.Exists("").ReturnsForAnyArgs(true); //all directories exist

            var directoryHelper = Substitute.ForPartsOf<DirectoryHelper>(_fullDirectoryInfoMock, _directoryMock);
            directoryHelper.CreatedDirectories.Returns(_createdDirectoriesWith3Elements);

            var result = directoryHelper.DeleteCreatedDirectories();

            Assert.IsFalse(result);
        }
    }
}
