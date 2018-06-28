using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using SystemInterface.IO;

namespace Presentation.UnitTest.Helper
{
    [TestFixture]
    public class LastSaveDirectoryHelperTest
    {
        private ILastSaveDirectoryHelper _lastSaveDirectoryHelper;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private PdfCreatorSettings _currentSettings;
        private ISettingsManager _settingsManager;
        private ITempFolderProvider _tempFolderProvider;
        private Job _job;
        private const string SomeFilename = "SomeFilename.pdf";
        private const string OutputDirectory = "OutputDirectory";
        private const string LastSaveDirectory = "LastSaveDirectory";
        private const string TempFolder = "Tempfolder";
        private readonly string _outputFilenameTemplate = Path.Combine(OutputDirectory, SomeFilename);

        [SetUp]
        public void SetUp()
        {
            _currentSettings = new PdfCreatorSettings(null);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(_currentSettings);

            _settingsManager = Substitute.For<ISettingsManager>();

            _tempFolderProvider = Substitute.For<ITempFolderProvider>();
            _tempFolderProvider.TempFolder.Returns(TempFolder);

            var pathUtil = new PathUtil(Substitute.For<IPath>(), Substitute.For<IDirectory>());

            _lastSaveDirectoryHelper = new LastSaveDirectoryHelper(_currentSettingsProvider, _settingsManager, _tempFolderProvider, pathUtil);

            _job = new Job(new JobInfo(), new ConversionProfile(), new JobTranslations(), new Accounts());
            _job.OutputFilenameTemplate = _outputFilenameTemplate;
        }

        [Test]
        public void Apply_ProfileHasTargetDirectory_JobOutputFilenameTemplateRemains()
        {
            _job.Profile.TargetDirectory = "Not Empty";

            _lastSaveDirectoryHelper.Apply(_job);

            Assert.AreEqual(_outputFilenameTemplate, _job.OutputFilenameTemplate);
        }

        [Test]
        public void Apply_ProfileHasNoTargetDirectory_SettingsLastSaveDierectoryIsEmpty_JobOutputFilenameTemplateRemains()
        {
            _job.Profile.TargetDirectory = string.Empty;
            _currentSettings.ApplicationSettings.LastSaveDirectory = string.Empty;

            _lastSaveDirectoryHelper.Apply(_job);

            Assert.AreEqual(_outputFilenameTemplate, _job.OutputFilenameTemplate);
        }

        [Test]
        public void Apply_ProfileHasTargetDirectory_SettingsLastSaveDierectoryIsNotEmpty_JobOutputFilenameRelatesToLastSaveDirectory()
        {
            _job.Profile.TargetDirectory = string.Empty;
            _currentSettings.ApplicationSettings.LastSaveDirectory = LastSaveDirectory;

            _lastSaveDirectoryHelper.Apply(_job);

            Assert.AreEqual(Path.Combine(LastSaveDirectory, SomeFilename), _job.OutputFilenameTemplate);
        }

        [Test]
        public void Apply_ProfileHasAVeryLongFileName_NoException()
        {
            _job.OutputFilenameTemplate = "X:\\" + new string('a', 300) + ".pdf";
            _currentSettings.ApplicationSettings.LastSaveDirectory = LastSaveDirectory;

            Assert.DoesNotThrow(() => _lastSaveDirectoryHelper.Apply(_job));
        }

        [Test]
        public void Save_ProfileHasTargetDirectory_LastSaveDirectoryDoesNotGetSetAndNotSaved()
        {
            _job.Profile.TargetDirectory = "Not empty";
            _lastSaveDirectoryHelper.Save(_job);

            _currentSettingsProvider.DidNotReceive();
            _settingsManager.DidNotReceive().SaveCurrentSettings();
        }

        [Test]
        public void Save_ProfileHasNoTargetDirectory_LastSaveDirectoryGetsSetAndSaved()
        {
            _job.Profile.TargetDirectory = String.Empty;
            _job.OutputFilenameTemplate = Path.Combine(LastSaveDirectory, SomeFilename);
            _lastSaveDirectoryHelper.Save(_job);

            Received.InOrder(() =>
            {
                _currentSettingsProvider.Received();
                _settingsManager.Received().SaveCurrentSettings();
            });

            _settingsManager.Received(1).SaveCurrentSettings();
            Assert.AreEqual(LastSaveDirectory, _currentSettings.ApplicationSettings.LastSaveDirectory, "LastSaveDirectory was not set");
        }

        [Test]
        public void Save_ProfileHasNoTargetDirectoryButOutputFileNameTemplateStartsWithTempfolder_LastSaveDirectoryDoesNotGetSetAndNotSaved()
        {
            _job.Profile.TargetDirectory = String.Empty;
            var tempFolder = Path.Combine(TempFolder, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            _job.OutputFilenameTemplate = Path.Combine(tempFolder, SomeFilename);
            _lastSaveDirectoryHelper.Save(_job);

            _currentSettingsProvider.DidNotReceive();
            _settingsManager.DidNotReceive().SaveCurrentSettings();
        }
    }
}
