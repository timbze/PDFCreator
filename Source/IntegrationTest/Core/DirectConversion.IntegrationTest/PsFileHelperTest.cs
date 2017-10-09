using NSubstitute;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.IO;
using System.Linq;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Core.DirectConversion
{
    [TestFixture]
    internal class PsFileHelperTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("PsFileHelperTest");
            _testSpoolfolder = Path.Combine(_th.TmpTestFolder, "TestSpoolFolder");
            var spoolerProvider = Substitute.For<ISpoolerProvider>();
            spoolerProvider.SpoolFolder.Returns(_testSpoolfolder);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _psTestFile = _th.TmpPsFiles[0];

            var settingsProvider = Substitute.For<ISettingsProvider>();
            var settings = new PdfCreatorSettings(Substitute.For<IStorage>());
            settingsProvider.Settings.Returns(settings);

            _directConversionBase = new PsDirectConversion(settingsProvider, new JobInfoManager(null), spoolerProvider, new FileWrap(), new DirectoryWrap(), new PathWrapSafe());
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;
        private string _testSpoolfolder;
        private string _psTestFile;
        private DirectConversionBase _directConversionBase;

        [Test]
        public void TransfrormToInfFile_InfFileBasicCheck()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, _testSpoolfolder);
            Assert.IsTrue(File.Exists(infFile), "Inf file does not exist");

            StringAssert.StartsWith(_testSpoolfolder, infFile, "Inf file not in spool folder.");
            StringAssert.EndsWith(Path.GetFileName(_psTestFile), Path.GetDirectoryName(infFile), "Inf file not in folder named after ps file");
            Assert.AreEqual(".inf", Path.GetExtension(infFile), "Inf file has wrong extension.");
            Assert.AreEqual(Path.GetFileName(_psTestFile), Path.GetFileNameWithoutExtension(infFile), "Inf file not named after ps file");
            Assert.IsNotEmpty(File.ReadAllText(infFile), "Inf file is empty");
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsFileWithDirectoryAsDocumentTitle()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("DocumentTitle=" + _psTestFile, content);
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsPsFileInSpoolFolder()
        {
            var psFilename = Path.GetFileName(_psTestFile);
            var psFileInSpoolFolder = Path.Combine(_testSpoolfolder, psFilename);
            psFileInSpoolFolder = Path.Combine(psFileInSpoolFolder, psFilename);

            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("SpoolFileName=" + psFileInSpoolFolder, content);
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsSourceFileTypePs()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("SourceFileType=ps", content);
        }

        [Test]
        public void TransfrormToInfFile_PsFileIsEmpty_ReturnsEmptyString()
        {
            Assert.IsEmpty(_directConversionBase.TransformToInfFile("", "something"));
        }

        [Test]
        public void TransfrormToInfFile_PsFileIsNull_ReturnsEmptyString()
        {
            Assert.IsEmpty(_directConversionBase.TransformToInfFile(null, "something"));
        }

        [Test]
        public void TransfrormToInfFile_SpoolFolderContainsFolderWithPsFileName()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, _testSpoolfolder);
            var spoolDirectories = Directory.GetDirectories(_testSpoolfolder);
            var psFileName = Path.GetFileName(_psTestFile);
            var psDir = spoolDirectories.First(x => x.EndsWith(psFileName)); //Throws Exception if element does not exist
        }

        [Test]
        public void TransfrormToInfFile_WithoutPrinter_PrinterNameInPsFileIsPDFCreator()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("PrinterName=PDFCreator", content);
        }

        [Test]
        public void TransfrormToInfFile_WithPrinter_PrinterNameInPsFileIsPDFCreator()
        {
            var infFile = _directConversionBase.TransformToInfFile(_psTestFile, "SomePrinerName");

            var content = File.ReadAllLines(infFile);
            Assert.Contains("PrinterName=SomePrinerName", content);
        }
    }
}
