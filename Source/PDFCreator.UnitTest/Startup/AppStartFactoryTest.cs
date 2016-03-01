using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Startup;

namespace PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class AppStartFactoryTest
    {
        [Test]
        public void Called_WithEmptyCommandLine_ReturnsMainWindowStartUp()
        {
            string[] args = { };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<MainWindowStart>(appStart);
        }

        [Test]
        public void Called_WithNonSenseParameter_ReturnsMainWindowStartUp()
        {
            string[] args = { "/NonSenseParameter123" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<MainWindowStart>(appStart);
        }

        [Test]
        public void Called_WithNonSense_ReturnsMainWindowStartUp()
        {
            string[] args = { "NonSense123" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<MainWindowStart>(appStart);
        }

        [Test]
        public void Called_WithNotExistingFile_ReturnsMainWindowStartUp()
        {
            string[] args = { "NotExistingFile.xxx" };

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("NotExistingFile.xxx").Returns(false);
            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args, fileStub);

            Assert.IsAssignableFrom<MainWindowStart>(appStart);
        }

        [Test]
        public void Called_WithExistingFile_ReturnsDragAndDropStart()
        {
            string[] args = { "ExistingFile.xxx" };

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("ExistingFile.xxx").Returns(true);
            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args, fileStub);

            Assert.IsAssignableFrom<DragAndDropStart>(appStart);
        }

        [Test]
        public void Called_MultipleExistingFiles_ReturnsDragAndDropStart()
        {
            string[] args = { "ExistingFile1.xxx", "ExistingFile2.xxx", "ExistingFile3.xxx" };

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("ExistingFile1.xxx").Returns(true);
            fileStub.Exists("ExistingFile2.xxx").Returns(true);
            fileStub.Exists("ExistingFile3.xxx").Returns(true);
            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args, fileStub);

            Assert.IsAssignableFrom<DragAndDropStart>(appStart);
        }

        [Test]
        public void Called_ExistingAndNotExistingFiles_ReturnsDragAndDropStart()
        {
            string[] args = { "ExistingFile1.xxx", "NotExistingFile.xxx", "ExistingFile2.xxx" };

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("ExistingFile1.xxx").Returns(true);
            fileStub.Exists("NotExistingFile.xxx").Returns(false);
            fileStub.Exists("ExistingFile2.xxx").Returns(true);
            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args, fileStub);

            Assert.IsAssignableFrom<DragAndDropStart>(appStart);
        }

        [Test]
        public void Called_WithFilesAndNonsenseParameter_ReturnsMainWindowStart()
        {
            string[] args = { "ExistingFile1.xxx", "/NonSenseParameter", "NotExistingFile.xxx", "ExistingFile2.xxx" };

            var fileStub = Substitute.For<IFile>();
            fileStub.Exists("ExistingFile1.xxx").Returns(true);
            fileStub.Exists("NotExistingFile.xxx").Returns(false);
            fileStub.Exists("ExistingFile2.xxx").Returns(true);
            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args, fileStub);

            Assert.IsAssignableFrom<MainWindowStart>(appStart);
        }

        [Test]
        public void Called_WithManagePrintJobs_ReturnsManagePrintJobsStartUp()
        {
            string[] args = { "/ManagePrintJobs" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            MaybePipedStart maybePipedStart = appStart as MaybePipedStart;

            Assert.IsTrue(maybePipedStart.StartManagePrintJobs);
        }

        [Test]
        public void Called_WithInitializeSettings_ReturnsInitializeJobStart()
        {
            string[] args = { string.Format("/InitializeSettings") };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<InitializeSettingsStart>(appStart);
        }

        [Test]
        public void Called_WithPrintFileParameter_ReturnsPrintFileStartUp()
        {
            string[] args = { "/PrintFile=C:\\Test.txt" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("C:\\Test.txt", ((PrintFileStart)appStart).PrintFile);
        }

        [Test]
        public void Called_WithPrintFileAndPrinterParameter_ReturnsPrintFileStartUpWithCorrectPrinter()
        {
            string[] args = { "/PrintFile=C:\\Test.txt", "/PrinterName=TestPrinter" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("TestPrinter", ((PrintFileStart)appStart).PrinterName);
        }

        [Test]
        public void Called_WithPrintFileAndInitializeSettings_ReturnsPrintFileJobStart()
        {
            string[] args = { string.Format("/PrintFile=C:\\Test.txt"), "/InitializeSettings" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
        }

        [Test]
        public void Called_WithJobInfo_ReturnsNewPrintJobStart()
        {
            string jobFile = @"C:\test.inf";
            string[] args = { string.Format("/InfoDataFile={0}", jobFile) };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewPrintJobStart)appStart).NewJobInfoFile, "Wrong JobInfoFile.");
        }

        [Test]
        public void Called_WithJobInfoAndInitializeSettings_ReturnsNewPrintJobStart()
        {
            string jobFile = @"C:\test.inf";
            string[] args = { string.Format("/InfoDataFile={0}", jobFile), "/InitializeSettings" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
        }

        [Test]
        public void Called_WithJobInfoAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            string jobFile = @"C:\test.inf";
            string[] args = { string.Format("/InfoDataFile={0}", jobFile), "/ManagePrintJobs" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.IsTrue(((NewPrintJobStart)appStart).StartManagePrintJobs);
        }

        [Test]
        public void Called_WithPsFile_ReturnsNewPsJobStart()
        {
            string jobFile = @"C:\test.ps";
            string[] args = { string.Format("/PsFile={0}", jobFile) };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewPsJobStart)appStart).NewPsFile, "Wrong File in NewPsJobStart.");
        }

        [Test]
        public void Called_WithPsFileAndInitializeSettings_ReturnsNewPsJobStart()
        {
            string jobFile = @"C:\test.ps";
            string[] args = { string.Format("/PsFile={0}", jobFile), "/InitializeSettings" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
        }

        [Test]
        public void Called_WithPsFileAndManagePrintJobs_NewPsJobStartWithManagePrintJobs()
        {
            string jobFile = @"C:\test.ps";
            string[] args = { string.Format("/PsFile={0}", jobFile), "/ManagePrintJobs" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.IsTrue(((NewPsJobStart)appStart).StartManagePrintJobs);
        }

        [Test]
        public void Called_WithPdfFile_ReturnsNewPsJobStart()
        {
            string jobFile = @"C:\test.pdf";
            string[] args = { string.Format("/PdfFile={0}", jobFile) };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewPsJobStart)appStart).NewPsFile, "Wrong File in NewPsJobStart.");
        }

        [Test]
        public void Called_WithPdfFileAndInitializeSettings_ReturnsNewPrintJobStart()
        {
            string jobFile = @"C:\test.pdf";
            string[] args = { string.Format("/PdfFile={0}", jobFile), "/InitializeSettings" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
        }
        
        [Test]
        public void Called_WithPdfFileAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            string jobFile = @"C:\test.pdf";
            string[] args = { string.Format("/PdfFile={0}", jobFile), "/ManagePrintJobs" };

            var appStartFactory = new AppStartFactory(new ApplicationSettings());
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.IsTrue(((NewPsJobStart)appStart).StartManagePrintJobs);
        }
    }
}