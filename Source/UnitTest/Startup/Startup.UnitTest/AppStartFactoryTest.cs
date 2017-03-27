using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class AppStartFactoryTest
    {
        [SetUp]
        public void Setup()
        {
            LoggingHelper.InitConsoleLogger("Test", LoggingLevel.Off);

            var startupConditions = Substitute.For<ICheckAllStartupConditions>();
            var starter = Substitute.For<IMaybePipedApplicationStarter>();
            starter.StartupConditions.Returns(startupConditions);

            _resolver = Substitute.For<IAppStartResolver>();
            // We need some special syntax here to make NSubsitute work here: .Returns(x => new MainWindowStart(...));
            _resolver.ResolveAppStart<MainWindowStart>().Returns(x => new MainWindowStart(null, starter, Substitute.For<IPdfArchitectCheck>(), Substitute.For<IMainWindowThreadLauncher>()));
            _resolver.ResolveAppStart<DragAndDropStart>().Returns(x => new DragAndDropStart(Substitute.For<IFileConversionHandler>(), starter));
            _resolver.ResolveAppStart<PrintFileStart>().Returns(x => new PrintFileStart(Substitute.For<ICheckAllStartupConditions>(), Substitute.For<IPrintFileHelper>(), null));
            _resolver.ResolveAppStart<NewPsJobStart>().Returns(x => new NewPsJobStart(null, null, starter, null, null));
            _resolver.ResolveAppStart<NewPdfJobStart>().Returns(x => new NewPdfJobStart(null, null, starter, null, null));
            _resolver.ResolveAppStart<NewPrintJobStart>().Returns(x => new NewPrintJobStart(null, null, null, starter, null));
            _resolver.ResolveAppStart<InitializeDefaultSettingsStart>().Returns(x => new InitializeDefaultSettingsStart(null, null, null, Substitute.For<IInstallationPathProvider>(), Substitute.For<IDataStorageFactory>()));
            _resolver.ResolveAppStart<StoreLicenseForAllUsersStart>().Returns(x => new StoreLicenseForAllUsersStart(null, null, new InstallationPathProvider("", "", "")));

            // TODO: Add tests of thr Run() method as it is testable now
        }

        private IAppStartResolver _resolver;

        private AppStartFactory BuildAppStartFactory()
        {
            var appStartFactory = new AppStartFactory(_resolver);
            return appStartFactory;
        }

        [Test]
        public void Called_MultipleRootedFiles_ReturnsDragAndDropStart()
        {
            string[] args = {@"\RootedFile1.xxx", @"\RootedFile2.xxx", @"\RootedFile3.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DragAndDropStart>();
        }

        [Test]
        public void Called_RootedAndNotRootedFiles_ReturnsMainWindowStart()
        {
            string[] args = {@"\RootedFile1.xxx", @"NotRootedFile.xxx", @"\RootedFile2.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_RootedDirectory_ReturnsMainWindowStart()
        {
            string[] args = {@"\RootedDirectory"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_RootedFileAndRootedDirectory_ReturnsMainWindowStart()
        {
            string[] args = {@"\RootedDirectory", @"\RootedFile1.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithCompleteFilePath_ReturnsDragAndDropStart()
        {
            string[] args = {@"C:\RootedFile.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DragAndDropStart>();
        }

        [Test]
        public void Called_WithEmptyCommandLine_ReturnsMainWindowStartUp()
        {
            string[] args = {};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithFilesAndNonsenseParameter_ReturnsMainWindowStart()
        {
            string[] args = {@"\RootedFile1.xxx", "/NonSenseParameter", @"\RootedFile.xxx", @"\RootedFile2.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithInitializeSettings_ReturnsInitializeJobStart()
        {
            string[] args = {"-InitializeSettings"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<InitializeSettingsStart>();
        }

        [Test]
        public void Called_WithJobInfo_ReturnsNewPrintJobStart()
        {
            var jobFile = @"C:\test.inf";
            string[] args = {$"/InfoDataFile={jobFile}"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewPrintJobStart) appStart).NewJobInfoFile, "Wrong JobInfoFile.");
        }

        [Test]
        public void Called_WithJobInfoAndInitializeSettings_ReturnsNewPrintJobStart()
        {
            var jobFile = @"C:\test.inf";
            string[] args = {$"/InfoDataFile={jobFile}", "/InitializeSettings"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
        }

        [Test]
        public void Called_WithJobInfoAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.inf";
            string[] args = {$"-InfoDataFile={jobFile}", "-ManagePrintJobs"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.IsTrue(((NewPrintJobStart) appStart).StartManagePrintJobs);
        }

        [Test]
        public void Called_WithManagePrintJobs_ReturnsManagePrintJobsStartUp()
        {
            string[] args = {"/ManagePrintJobs"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            var maybePipedStart = appStart as MaybePipedStart;

            Assert.IsTrue(maybePipedStart.StartManagePrintJobs);
        }

        [Test]
        public void Called_WithNonSense_ReturnsMainWindowStartUp()
        {
            string[] args = {"NonSense123"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNonSenseParameter_ReturnsMainWindowStartUp()
        {
            string[] args = {"/NonSenseParameter123"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNonSenseParameterHyphen_ReturnsMainWindowStartUp()
        {
            string[] args = {"-NonSenseParameter123"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNotRootedFilePath_ReturnsMainWindowStart()
        {
            string[] args = {"NotRootedFile.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithPdfFile_ReturnsNewPsJobStart()
        {
            var jobFile = @"C:\test.pdf";
            string[] args = {$"/PdfFile={jobFile}"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPdfJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewDirectConversionJobStart) appStart).NewDirectConversionFile, "Wrong File in NewDirectConversionJobStart.");
        }

        [Test]
        public void Called_WithPdfFileAndInitializeSettings_ReturnsNewPrintJobStart()
        {
            var jobFile = @"C:\test.pdf";
            string[] args = {$"/PdfFile={jobFile}", "/InitializeSettings"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPdfJobStart>(appStart);
        }

        [Test]
        public void Called_WithPdfFileAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.pdf";
            string[] args = {$"/PdfFile={jobFile}", "/ManagePrintJobs"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPdfJobStart>(appStart);
            Assert.IsTrue(((NewDirectConversionJobStart) appStart).StartManagePrintJobs);
        }

        [Test]
        public void Called_WithPrintFileAndInitializeSettings_ReturnsPrintFileJobStart()
        {
            string[] args = {"-PrintFile=C:\\Test.txt", "-InitializeSettings"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
        }

        [Test]
        public void Called_WithPrintFileAndPrinterParameter_ReturnsPrintFileStartUpWithCorrectPrinter()
        {
            string[] args = {"/PrintFile=C:\\Test.txt", "/PrinterName=TestPrinter"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("TestPrinter", ((PrintFileStart) appStart).PrinterName);
        }

        [Test]
        public void Called_WithPrintFileParameter_ReturnsPrintFileStartUp()
        {
            string[] args = {"/PrintFile=C:\\Test.txt"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("C:\\Test.txt", ((PrintFileStart) appStart).PrintFile);
        }

        [Test]
        public void Called_WithPsFile_ReturnsNewPsJobStart()
        {
            var jobFile = @"C:\test.ps";
            string[] args = {$"/PsFile={jobFile}"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewDirectConversionJobStart) appStart).NewDirectConversionFile, "Wrong File in NewDirectConversionJobStart.");
        }

        [Test]
        public void Called_WithPsFileAndInitializeSettings_ReturnsNewPsJobStart()
        {
            var jobFile = @"C:\test.ps";
            string[] args = {$"/PsFile={jobFile}", "/InitializeSettings"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
        }

        [Test]
        public void Called_WithPsFileAndManagePrintJobs_NewPsJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.ps";
            string[] args = {$"/PsFile={jobFile}", "/ManagePrintJobs"};

            var appStartFactory = BuildAppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPsJobStart>(appStart);
            Assert.IsTrue(((NewDirectConversionJobStart) appStart).StartManagePrintJobs);
        }

        [Test]
        public void Called_WithRootedFilePath_ReturnsDragAndDropStart()
        {
            string[] args = {@"\RootedFile.xxx"};

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DragAndDropStart>();
        }

        [Test]
        public void Called_WithInitializeCustomSettings_ReturnsInitializeCustomSettingsStart()
        {
            string[] args = { @"/InitializeDefaultSettings=SettingsFile.ini" };
            var appStartFactory = BuildAppStartFactory();

            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<InitializeDefaultSettingsStart>(appStart);
            Assert.AreEqual("SettingsFile.ini", ((InitializeDefaultSettingsStart)appStart).SettingsFile);
        }

        [Test]
        public void Called_WithInitializeCustomSettingsWithoutSettingsFile_ReturnsInitializeCustomSettingsStartWithSettingsFileNull()
        {
            string[] args = { @"/InitializeDefaultSettings" };
            var appStartFactory = BuildAppStartFactory();

            var appStart = appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<InitializeDefaultSettingsStart>(appStart);
            Assert.AreEqual(null, ((InitializeDefaultSettingsStart)appStart).SettingsFile);
        }

        [Test]
        public void Called_WithStoreLicenseForAllUsers_ReturnsStoreLicenseForAllUsersStart()
        {
            string[] args = { @"/StoreLicenseForAllUsers" };

            var appStartFactory = BuildAppStartFactory();
            appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<StoreLicenseForAllUsersStart>();
        }

        [Test]
        public void Called_WithStoreLicenseForAllUsersWithLicenseServerCode_AppStartContainsCode()
        {
            var code = "abcdefg";
            var licenseKey = "MY-KEY";

            string[] args = { @"/StoreLicenseForAllUsers", @"/LicenseServerCode=" + code, @"/LicenseKey=" + licenseKey };

            var appStartFactory = BuildAppStartFactory();
            var appStart = (StoreLicenseForAllUsersStart) appStartFactory.CreateApplicationStart(args);

            Assert.AreEqual(code, appStart.LicenseServerCode);
            Assert.AreEqual(licenseKey, appStart.LicenseKey);
        }
    }
}