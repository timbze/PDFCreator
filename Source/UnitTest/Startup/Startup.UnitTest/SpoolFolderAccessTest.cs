using System;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Core.Startup.StartConditions;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class SpoolFolderAccessTest
    {
        [SetUp]
        public void Setup()
        {
            _spoolerProvider = Substitute.For<ISpoolerProvider>();
            _spoolerProvider.SpoolFolder.Returns(SpoolFolder);

            _directory = Substitute.For<IDirectory>();
            _directory.EnumerateDirectories(SpoolFolder, "*", SearchOption.AllDirectories)
                .Returns(x => _spoolFolderSubdirs);

            _spoolFolderSubdirs = new List<string>();
        }

        private ISpoolerProvider _spoolerProvider;
        private IDirectory _directory;
        private const string SpoolFolder = "X:\\Temp\\Spool";
        private List<string> _spoolFolderSubdirs;

        private SpoolFolderAccess BuildSpoolFolderAccess()
        {
            return new SpoolFolderAccess(_spoolerProvider, _directory);
        }

        private void AddAccessibleDirectory(string path)
        {
            _directory.Exists(path).Returns(true);
            if (path.StartsWith(SpoolFolder + "\\"))
                _spoolFolderSubdirs.Add(path);
        }

        private void AddInaccessibleDirectory(string path, bool genericException = false)
        {
            _directory.Exists(path).Returns(true);

            var exception = genericException ? new Exception() : new UnauthorizedAccessException();

            _directory.GetAccessControl(path).Returns(x => { throw exception; });
            if (path.StartsWith(SpoolFolder + "\\"))
                _spoolFolderSubdirs.Add(path);
        }

        [Test]
        public void WhenSpoolFolderDoesNotExist_FolderCannotBeCreated_Fails()
        {
            var spoolFolderAccess = BuildSpoolFolderAccess();

            _directory.CreateDirectory(SpoolFolder).Returns(x => { throw new UnauthorizedAccessException(); });

            var success = spoolFolderAccess.CanAccess();

            _directory.Received().CreateDirectory(SpoolFolder);
            Assert.IsFalse(success);
        }

        [Test]
        public void WhenSpoolFolderDoesNotExist_FolderIsCreated_Success()
        {
            var spoolFolderAccess = BuildSpoolFolderAccess();

            var success = spoolFolderAccess.CanAccess();

            _directory.Received().CreateDirectory(SpoolFolder);
            Assert.IsTrue(success);
        }

        [Test]
        public void WhenSpoolFolderIsEmptyAndAccessible_Success()
        {
            var spoolFolderAccess = BuildSpoolFolderAccess();
            AddAccessibleDirectory(SpoolFolder);

            var success = spoolFolderAccess.CanAccess();

            _directory.DidNotReceive().CreateDirectory(SpoolFolder);
            Assert.IsTrue(success);
        }

        [Test]
        public void WhenSpoolFolderNotEmptyAndAccessible_Success()
        {
            var paths = new[]
            {
                SpoolFolder,
                Path.Combine(SpoolFolder, "Folder1"),
                Path.Combine(SpoolFolder, "Folder2")
            };

            var spoolFolderAccess = BuildSpoolFolderAccess();

            foreach (var path in paths)
                AddAccessibleDirectory(path);

            var success = spoolFolderAccess.CanAccess();

            foreach (var path in paths)
                _directory.Received().GetAccessControl(path);

            Assert.IsTrue(success);
        }

        [Test]
        public void WithGenericExceptionWhileQueryingSubfolder_Success()
        {
            var spoolFolderAccess = BuildSpoolFolderAccess();

            AddAccessibleDirectory(SpoolFolder);
            AddInaccessibleDirectory(Path.Combine(SpoolFolder, "BrokenFolder"), true);

            var success = spoolFolderAccess.CanAccess();

            Assert.IsTrue(success);
        }

        [Test]
        public void WithInaccessableSubfolder_Fails()
        {
            var spoolFolderAccess = BuildSpoolFolderAccess();

            AddAccessibleDirectory(SpoolFolder);
            AddInaccessibleDirectory(Path.Combine(SpoolFolder, "BrokenFolder"));

            var success = spoolFolderAccess.CanAccess();

            Assert.IsFalse(success);
        }
    }
}
