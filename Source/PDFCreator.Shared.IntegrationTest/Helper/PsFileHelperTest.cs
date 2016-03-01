using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;
using PDFCreator.TestUtilities;

namespace PDFCreator.Shared.IntegrationTest.Helper
{
    [TestFixture]
    class PsFileHelperTest
    {
        private TestHelper _th;
        private string _testSpoolfolder;
        private string _psTestFile;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper("PsFileHelperTest");
            _testSpoolfolder = Path.Combine(_th.TmpTestFolder, "TestSpoolFolder");
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _psTestFile = _th.TmpPsFiles[0];
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void TransfrormToInfFile_PsFileIsNull_ReturnsEmptyString()
        {
            Assert.IsEmpty(PsFileHelper.TransformToInfFile(null, "something"));
        }

        [Test]
        public void TransfrormToInfFile_PsFileIsEmpty_ReturnsEmptyString()
        {
            Assert.IsEmpty(PsFileHelper.TransformToInfFile("", "something"));
        }

        [Test]
        public void TransfrormToInfFile_SpoolFolderContainsFolderWithPsFileName()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);
            var spoolDirectories = Directory.GetDirectories(_testSpoolfolder);
            var psFileName = Path.GetFileName(_psTestFile);
            var psDir = spoolDirectories.First(x => x.EndsWith(psFileName)); //Throws Exception if element does not exist
        }

        [Test]
        public void TransfrormToInfFile_InfFileBasicCheck()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);
            Assert.IsTrue(File.Exists(infFile), "Inf file does not exist");

            StringAssert.StartsWith(_testSpoolfolder, infFile, "Inf file not in spool folder.");
            StringAssert.EndsWith(Path.GetFileName(_psTestFile), Path.GetDirectoryName(infFile), "Inf file not in folder named after ps file");
            Assert.AreEqual(".inf", Path.GetExtension(infFile), "Inf file has wrong extension.");
            Assert.AreEqual(Path.GetFileName(_psTestFile), Path.GetFileNameWithoutExtension(infFile), "Inf file not named after ps file");
            Assert.IsNotEmpty(File.ReadAllText(infFile), "Inf file is empty");
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsPsFileInSpoolFolder()
        {
            var psFilename = Path.GetFileName(_psTestFile);
            var psFileInSpoolFolder = Path.Combine(_testSpoolfolder, psFilename);
            psFileInSpoolFolder = Path.Combine(psFileInSpoolFolder, psFilename);

            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("SpoolFileName=" + psFileInSpoolFolder,  content);
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsFileWithDirectoryAsDocumentTitle()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("DocumentTitle=" + _psTestFile, content);
        }

        [Test]
        public void TransfrormToInfFile_WithoutPrinter_PrinterNameInPsFileIsPDFCreator()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("PrinterName=PDFCreator", content);
        }

        [Test]
        public void TransfrormToInfFile_WithPrinter_PrinterNameInPsFileIsPDFCreator()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder, "SomePrinerName");

            var content = File.ReadAllLines(infFile);
            Assert.Contains("PrinterName=SomePrinerName", content);
        }

        [Test]
        public void TransfrormToInfFile_InfFileContainsSourceFileTypePs()
        {
            var infFile = PsFileHelper.TransformToInfFile(_psTestFile, _testSpoolfolder);

            var content = File.ReadAllLines(infFile);
            Assert.Contains("SourceFileType=ps", content);
        }
    }
}
