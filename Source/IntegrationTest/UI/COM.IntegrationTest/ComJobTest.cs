using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.COM;
using pdfforge.PDFCreator.UI.Presentation.Commands.FirstTimeCommands;
using pdfforge.PDFCreator.UI.Presentation.Settings;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using Translatable;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class ComJobTest
    {
        [OneTimeSetUp]
        public void CleanDependencies()
        {
            ComDependencyBuilder.ResetDependencies();
            ComTestHelper.ModifyAndBuildComDependencies();
        }

        [SetUp]
        public void SetUp()
        {
            var dependencies = ComTestHelper.ModifyAndBuildComDependencies();

            LoggingHelper.InitConsoleLogger("PDFCreatorTest", LoggingLevel.Off);

            var installationPathProvider = new InstallationPathProvider(@"Software\pdfforge\PDFCreator\Settings", @"Software\pdfforge\PDFCreator", "{00000000-0000-0000-0000-000000000000}");

            var settingsProvider = new DefaultSettingsProvider();

            var translationHelper = new TranslationHelper(settingsProvider, new AssemblyHelper(GetType().Assembly), new TranslationFactory(), null);
            translationHelper.InitTranslator("None");

            var settingsLoader = new SettingsLoader(translationHelper, Substitute.For<ISettingsMover>(), installationPathProvider, Substitute.For<IPrinterHelper>());

            var settingsManager = new SettingsManager(settingsProvider, settingsLoader, installationPathProvider, new VersionHelper(Assembly.GetExecutingAssembly()), new List<IFirstTimeCommand>(), Substitute.For<ICommandLocator>());
            settingsManager.LoadAllSettings();

            var folderProvider = new FolderProvider(new PrinterPortReader(new RegistryWrap(), new PathWrapSafe()), new PathWrap());

            _queue = new Queue();
            _queue.Initialize();

            _testPageHelper = new TestPageHelper(new VersionHelper(GetType().Assembly), new OsHelper(), folderProvider,
                dependencies.QueueAdapter.JobInfoQueue, new JobInfoManager(new LocalTitleReplacerProvider(new List<TitleReplacement>())), new ApplicationNameProvider("FREE"));
        }

        [TearDown]
        public void TearDown()
        {
            _queue.Clear();
            _queue.ReleaseCom();
        }

        private Queue _queue;
        private TestPageHelper _testPageHelper;

        private void CreateTestPages(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _testPageHelper.CreateTestPage();
            }
        }

        [Test]
        public void ConvertTo_IfFilenameDirectoryNotExisting_ThrowsCOMException()
        {
            CreateTestPages(1);

            const string filename = "basdeead\\aokdeaad.pdf";
            var comJob = _queue.NextJob;

            var ex = Assert.Throws<COMException>(() => comJob.ConvertTo(filename));
            StringAssert.Contains("Invalid path. Please check if the directory exists.", ex.Message);
        }

        [Test]
        public void ConvertTo_IfFilenameEmpty_ThrowsArgumentException()
        {
            CreateTestPages(1);

            const string filename = "";
            var comJob = _queue.NextJob;

            Assert.Throws<ArgumentException>(() => comJob.ConvertTo(filename));
        }

        [Test]
        public void ConvertTo_IfFilenameHasIllegalChars_ThrowsArgumentException()
        {
            CreateTestPages(1);

            const string filename = "testpage>testpage.pdf";
            var comJob = _queue.NextJob;

            Assert.Throws<ArgumentException>(() => comJob.ConvertTo(filename));
        }

        [Test]
        public void GetProfileSettings_IfEmptyPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var propertyName = "";
            var ex = Assert.Throws<COMException>(() => comJob.GetProfileSetting(propertyName));
            StringAssert.Contains($"The property '{propertyName}' does not exist!", ex.Message);
        }

        [Test]
        public void GetProfileSettings_IfInvalidPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;

            var propertyName = "asdioajsd";
            var ex = Assert.Throws<COMException>(() => comJob.GetProfileSetting(propertyName));
            StringAssert.Contains($"The property '{propertyName}' does not exist!", ex.Message);
        }

        [Test]
        public void ProfileSettings_IfEmptyPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;

            var propertyName = "";
            var ex = Assert.Throws<COMException>(() => comJob.SetProfileSetting(propertyName, "True"));
            StringAssert.Contains($"The property '{propertyName}' does not exist!", ex.Message);
        }

        [Test]
        public void ProfileSettings_IfNotExistingPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var propertyName = "NotExisting";
            var ex = Assert.Throws<COMException>(() => comJob.SetProfileSetting(propertyName, "True"));
            StringAssert.Contains($"The property '{propertyName}' does not exist!", ex.Message);
        }

        [Test]
        public void SetProfileSettings_IfEmptyValue_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            Assert.Throws<FormatException>(() => comJob.SetProfileSetting("PdfSettings.Security.Enabled", ""));
        }

        [Test]
        public void SetProfileSettings_IfInappropriateValue_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            Assert.Throws<FormatException>(() => comJob.SetProfileSetting("PdfSettings.Security.Enabled", "3"));
        }
    }
}
