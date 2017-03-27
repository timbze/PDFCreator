using System;
using System.IO;
using SystemInterface.IO;
using SystemWrapper.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace pdfforge.PDFCreator.Core.DirectConversion.UnitTest
{

    abstract class DirectConversionTestBase
    {
        protected IFixture Fixture;
        protected IFile FileMock;
        private IDirectory _directoryMock;
        private ISpoolerProvider _spoolerProvider;
        private IJobInfoManager _jobInfoManagerMock;

        protected abstract DirectConversionBase BuildDirectConversion();
        protected abstract void ConfigureValidFileOpenRead(IFile file, string filename);

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            Fixture.Inject<IPathSafe>(new PathWrapSafe());
            FileMock = Fixture.Freeze<IFile>();
            _directoryMock = Fixture.Freeze<IDirectory>();
            _spoolerProvider = Fixture.Freeze<ISpoolerProvider>();
            _jobInfoManagerMock = Fixture.Freeze<IJobInfoManager>();
            var settingsProviderMock = Fixture.Freeze<ISettingsProvider>();
            settingsProviderMock.Settings.Returns(new PdfCreatorSettings(null));
            _spoolerProvider.SpoolFolder.Returns(Fixture.CreateFolderPath("SpoolFolder"));
            Fixture.Register(BuildDirectConversion);
        }

        [Test]
        public void TransformToInfFile_WithEmptyFilename_ReturnsEmptyString()
        {
            var directConversion = Fixture.Create<DirectConversionBase>();

            var infFile = directConversion.TransformToInfFile("");

            Assert.AreEqual("", infFile);
        }

        [Test]
        public void TransformToInfFile_WithNonExistingFileFilename_ReturnsEmptyString()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(false);
            var directConversion = Fixture.Create<DirectConversionBase>();

            var infFile = directConversion.TransformToInfFile(filename);

            Assert.AreEqual("", infFile);
        }

        [Test]
        public void TransformToInfFile_WithValidFile_ReturnsInfFilePath()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf", filename: "file");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();

            var infFile = directConversion.TransformToInfFile(filename, "MyPrinter");

            Assert.AreEqual($"{_spoolerProvider.SpoolFolder}\\file.pdf\\file.pdf.inf", infFile);
        }

        [Test]
        public void TransformToInfFile_WithValidFile_CreatesSpoolFolder()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf", filename: "file");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();

            directConversion.TransformToInfFile(filename, "MyPrinter");

            _directoryMock.Received(1).CreateDirectory($"{_spoolerProvider.SpoolFolder}\\file.pdf");
        }

        [Test]
        public void TransformToInfFile_WithValidFile_CopiesFileToSpoolFolder()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf", filename: "file");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();
            var spoolFolder = $"{_spoolerProvider.SpoolFolder}\\file.pdf";

            directConversion.TransformToInfFile(filename, "MyPrinter");

            FileMock.Received(1).Copy(filename, $"{spoolFolder}\\file.pdf");
        }

        [Test]
        public void TransformToInfFile_WithValidFile_CreatesInfFile()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf", filename: "file");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();
            var spoolFolder = $"{_spoolerProvider.SpoolFolder}\\file.pdf";

            directConversion.TransformToInfFile(filename, "MyPrinter");

            _jobInfoManagerMock.Received(1).SaveToInfFile(Arg.Any<JobInfo>(), $"{spoolFolder}\\file.pdf.inf");
        }

        [Test]
        public void TransformToInfFile_WithValidFile_UsesInfData()
        {
            var printerName = Fixture.Create("Printer");
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();

            directConversion.TransformToInfFile(filename, printerName);

            Predicate<JobInfo> validateJobInfo = jobInfo =>
            {
                Assert.AreEqual(1, jobInfo.SourceFiles.Count, "JobInfo was expected to contain one source file only!");
                Assert.AreEqual(printerName, jobInfo.SourceFiles[0].PrinterName, "The printer name was not applied!");
                return true;
            };

            _jobInfoManagerMock.Received(1).SaveToInfFile(Arg.Is<JobInfo>(x => validateJobInfo(x)), Arg.Any<string>());
        }

        [Test]
        public void TransformToInfFile_WithNoPrinterName_UsesPrimaryPrinter()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();

            directConversion.TransformToInfFile(filename);

            _jobInfoManagerMock.Received(1).SaveToInfFile(Arg.Is<JobInfo>(x => x.SourceFiles[0].PrinterName == "PDFCreator"), Arg.Any<string>());
        }

        [Test]
        public void TransformToInfFile_WithExceptionWhileCreatingJobFolder_ReturnsEmptyString()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();
            _directoryMock.When(x => x.CreateDirectory(Arg.Any<string>())).Throw<IOException>();

            var infFile = directConversion.TransformToInfFile(filename);

            Assert.AreEqual("", infFile);
        }

        [Test]
        public void TransformToInfFile_WithExceptionWhileCopying_ReturnsEmptyString()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();
            FileMock.When(x => x.Copy(Arg.Any<string>(), Arg.Any<string>())).Throw<IOException>();

            var infFile = directConversion.TransformToInfFile(filename);

            Assert.AreEqual("", infFile);
        }

        [Test]
        public void TransformToInfFile_WithExceptionWhileCreatingInfFile_ReturnsEmptyString()
        {
            var filename = Fixture.CreateFilePath(extension: "pdf");
            FileMock.Exists(filename).Returns(true);
            ConfigureValidFileOpenRead(FileMock, filename);
            var directConversion = Fixture.Create<DirectConversionBase>();
            _jobInfoManagerMock.When(x => x.SaveToInfFile(Arg.Any<JobInfo>(), Arg.Any<string>())).Throw<IOException>();

            var infFile = directConversion.TransformToInfFile(filename);

            Assert.AreEqual("", infFile);
        }
    }

    public static class FixturePathExtension
    {
        public static string CreateFolderPath(this IFixture fixture, string seed = null)
        {
            var folderName = fixture.Create(seed ?? "");

            return $"X:\\MockedPath\\{folderName}";
        }

        public static string CreateFilePath(this IFixture fixture, string extension, string filename = null)
        {
            if (filename == null)
                filename = fixture.Create("File");

            return $"{fixture.CreateFolderPath()}\\{filename}.{extension}";
        }
    }
}
