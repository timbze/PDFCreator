using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Shared.Helper;
using PDFCreator.TestUtilities;

namespace PDFCreator.Shared.IntegrationTest.Helper
{
    [TestFixture]
    class DirectoryHelperTest
    {
        private string _tempFolderBase;
        private string _fullDirectory;
        private string _firstParentDirectory;
        private string _secondParentDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempFolderBase = TempFileHelper.CreateTempFolder("DirectoryHelperTest");
            _secondParentDirectory = Path.Combine(_tempFolderBase, "secondparent");
            _firstParentDirectory = Path.Combine(_secondParentDirectory, "firstparent");
            _fullDirectory = Path.Combine(_firstParentDirectory, "directory");
        }

        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void CreateFullDirectory_CreatedDirectoryAndParentsExist()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();

            Assert.IsTrue(Directory.Exists(_fullDirectory), "full directory does not exist");
            Assert.IsTrue(Directory.Exists(_firstParentDirectory), "first parent directory does not exist");
            Assert.IsTrue(Directory.Exists(_secondParentDirectory), "second parent directory does not exist");
        }

        [Test]
        public void CreateFullDirectory_CreatedDirectoriesContainsOnlyFullDirectoryAndParents()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();

            Assert.AreEqual(3, dh.CreatedDirectories.Count);
            Assert.Contains(_fullDirectory, dh.CreatedDirectories);
            Assert.Contains(_firstParentDirectory, dh.CreatedDirectories);
            Assert.Contains(_secondParentDirectory, dh.CreatedDirectories);
        }

        [Test]
        public void CreateFullDirectoryTwice_CreatedDirectoriesContainsFullDirectoryAndParentsONLYonce()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            dh.CreateDirectory();

            Assert.AreEqual(3, dh.CreatedDirectories.Count);
            Assert.Contains(_fullDirectory, dh.CreatedDirectories);
            Assert.Contains(_firstParentDirectory, dh.CreatedDirectories);
            Assert.Contains(_secondParentDirectory, dh.CreatedDirectories);
        }

        [Test]
        public void CreateFullDirectoryAndDeleteCreatedDirectories_FullDirectoryAndParentsDoNotExist_TempFolderBaseRemains()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            dh.DeleteCreatedDirectories();

            Assert.IsFalse(Directory.Exists(_fullDirectory), "full directory still exists");
            Assert.IsFalse(Directory.Exists(_firstParentDirectory), "first parent directory still exists");
            Assert.IsFalse(Directory.Exists(_secondParentDirectory), "second parent directory still exists");
            Assert.IsTrue(Directory.Exists(_tempFolderBase), "Temp folder base was deleted");
        }

        [Test]
        public void CreateFullDirectoryAndAddFile_DeleteCreatedDirectories_FullDirectoryDoesNotGetDeleted()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            var fileInFullDirectory = Path.Combine(_fullDirectory, "file.tst");
            File.Create(fileInFullDirectory);
            dh.DeleteCreatedDirectories();

            Assert.IsTrue(Directory.Exists(_fullDirectory), "full directory does not exist");
            Assert.IsTrue(File.Exists(fileInFullDirectory), "file in full directory gets deleted.");
        }

        [Test]
        public void CreateFullDirectoryAndAddFileToFirstParent_DeleteCreatedDirectories_FirstParentAndFileStillExist()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            var fileInFirstParentDirectory = Path.Combine(_firstParentDirectory, "file.tst");
            File.Create(fileInFirstParentDirectory);
            dh.DeleteCreatedDirectories();

            Assert.IsFalse(Directory.Exists(_fullDirectory), "full directory still exists");
            Assert.IsTrue(File.Exists(fileInFirstParentDirectory), "file in full directory gets deleted.");
            Assert.IsTrue(Directory.Exists(_firstParentDirectory), "first parent directory does not exist");
        }

        [Test]
        public void CreateFullDirectoryDeleteCreatedDirectoriesCreateFullDirectoryDeleteCreatedDirectories_DoesNotThrowDirectoryNotFoundException()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            dh.DeleteCreatedDirectories();
            dh.CreateDirectory();
            dh.DeleteCreatedDirectories();
        }

        [Test]
        public void CreateFullDirectoryAndDeleteCreatedDirectoriesTwice_DoesNotThrowDirectoryNotFoundException()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            dh.DeleteCreatedDirectories();
            dh.DeleteCreatedDirectories();
        }

        [Test]
        public void CreateFullDirectoryDeleteItWithFirstParentCreateDirectoryAgainAndDeleteCreatedDirectories_DoesNotThrowDirectoryNotFoundException()
        {
            var dh = new DirectoryHelper(_fullDirectory);
            dh.CreateDirectory();
            Directory.Delete(_fullDirectory);
            Directory.Delete(_firstParentDirectory);
            dh.CreateDirectory();
          
            dh.DeleteCreatedDirectories();
        }
    }
}
