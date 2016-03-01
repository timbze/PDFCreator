using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.IO;
using PDFCreator.TestUtilities;

namespace PDFCreator.Utilities.IntegrationTest.IO
{
    [TestFixture]
    class FolderCleanerTest
    {
        private string _tempFolder;

        [SetUp]
        public void SetUp()
        {
            _tempFolder = TempFileHelper.CreateTempFolder("FolderCleanerTest");
        }

        [TearDown]
        public void TearDown()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void Clean_WithNonexistingPath_DoesNothing()
        {
            Directory.Delete(_tempFolder);

            var cleaner = new FolderCleaner(@"C:\SomeFolderThatDoesnotExist\With\Some\Sub\Folder");
            cleaner.Clean();
        }

        [Test]
        public void Clean_WithEmptyFolder_DoesNothing()
        {
            CreateTempFiles(folderName: _tempFolder, numberOfFiles: 6, age: TimeSpan.Zero);

            var cleaner = new FolderCleaner(_tempFolder);
            cleaner.Clean();

            Assert.IsTrue(Directory.Exists(_tempFolder), "The directory did not exist after cleaning up: " + _tempFolder);
        }

        [Test]
        public void Clean_WithSomeFiles_CleansAllFiles()
        {
            CreateTempFiles(folderName: _tempFolder, numberOfFiles: 4, age: TimeSpan.Zero);

            var cleaner = new FolderCleaner(_tempFolder);
            cleaner.Clean();

            Assert.IsTrue(Directory.Exists(_tempFolder), "The directory did not exist after cleaning up: " + _tempFolder);
            Assert.IsFalse(Directory.EnumerateFileSystemEntries(_tempFolder).Any(), "The directory is not empty after cleaning up: " + _tempFolder);
        }

        [Test]
        public void Clean_WithSomeFilesAndSubfolder_CleansAllFilesAndRemovesSubfolders()
        {
            CreateTempFiles(folderName: _tempFolder, numberOfFiles: 6, age: TimeSpan.Zero);
            var subFolder = Path.Combine(_tempFolder, "Subfolder");
            Directory.CreateDirectory(subFolder);
            CreateTempFiles(folderName: subFolder, numberOfFiles: 6, age: TimeSpan.Zero);

            var cleaner = new FolderCleaner(_tempFolder);
            cleaner.Clean();

            Assert.IsTrue(Directory.Exists(_tempFolder), "The directory did not exist after cleaning up: " + _tempFolder);
            Assert.IsFalse(Directory.EnumerateFileSystemEntries(_tempFolder).Any(), "The directory is not empty after cleaning up: " + _tempFolder);
        }

        [Test]
        public void CleanWithTimeSpan_WithSomeFiles_CleansAllFilesOlderThanTimeSpan()
        {
            CreateTempFiles(folderName: _tempFolder, numberOfFiles: 6, age: TimeSpan.FromDays(2));
            CreateSubFolderWithSomeFiles(numberOfFiles: 6, age: TimeSpan.FromDays(2));
            var files = CreateSubFolderWithSomeFiles(numberOfFiles: 1, age: TimeSpan.Zero);

            var cleaner = new FolderCleaner(_tempFolder);
            cleaner.Clean(TimeSpan.FromDays(1));

            Assert.IsTrue(Directory.Exists(_tempFolder), "The directory did not exist after cleaning up: " + _tempFolder);
            Assert.AreEqual(1, Directory.GetDirectories(_tempFolder).Count(), "There was more than one folder left");
            Assert.AreEqual(1, Directory.GetFiles(_tempFolder, "*", SearchOption.AllDirectories).Count(), "Found more than the one file after cleaning up");
            Assert.AreEqual(files.First(), Directory.GetFiles(_tempFolder, "*", SearchOption.AllDirectories).First());
        }


        private IList<string> CreateSubFolderWithSomeFiles(int numberOfFiles, TimeSpan age)
        {
            string folderName = Path.Combine(_tempFolder, Guid.NewGuid().ToString());
            Directory.CreateDirectory(folderName);

            return CreateTempFiles(folderName, numberOfFiles, age);
        }

        private IList<string> CreateTempFiles(string folderName, int numberOfFiles, TimeSpan age)
        {
            var files = new List<string>();

            for (int i = 0; i < numberOfFiles; i++)
            {
                var guid = Guid.NewGuid().ToString();
                var filename = Path.Combine(folderName, $"{guid}.tmp");
                File.WriteAllText(filename, $"Temp file {guid}");
                var fileTime = DateTime.Now - age;
                File.SetCreationTime(filename, fileTime);
                Assert.AreEqual(fileTime, File.GetCreationTime(filename));
                files.Add(filename);
            }

            return files;
        }
    }
}
