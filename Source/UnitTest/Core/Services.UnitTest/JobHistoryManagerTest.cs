using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest
{
    [TestFixture]
    public class JobHistoryManagerTest
    {
        private IJobHistoryManager _jobHistoryManager;
        private IJobHistoryStorage _jobHistoryStorage;
        private ITempFolderProvider _tempFolderProvider;
        private Job _job;
        private IList<HistoricJob> _storedHistory;
        private PdfCreatorSettings _settings;
        private IFile _file;
        private int _capacity = 10;
        private const string TempFolder = "TempFolder";

        [SetUp]
        public void SetUp()
        {
            _job = new Job(new JobInfo(), new ConversionProfile(), new JobTranslations(), new Accounts());
            _job.OutputFilenameTemplate = "MayNotStartWithTempfolder.pdf";
            _job.OutputFiles.Add("NotEmpty.pdf");
            _storedHistory = new List<HistoricJob>();

            _jobHistoryStorage = Substitute.For<IJobHistoryStorage>();
            for (var i = 0; i < _capacity; i++)
                _storedHistory.Add(new HistoricJob(_job));
            _jobHistoryStorage.Load().Returns(_storedHistory);

            var settingsProvider = Substitute.For<ISettingsProvider>();
            _settings = new PdfCreatorSettings(null);
            _settings.ApplicationSettings.JobHistory.Enabled = true;
            _settings.ApplicationSettings.JobHistory.Capacity = _capacity;
            settingsProvider.Settings.Returns(_settings);

            _tempFolderProvider = Substitute.For<ITempFolderProvider>();
            _tempFolderProvider.TempFolder.Returns(TempFolder);

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);

            _jobHistoryManager = new JobHistoryManager(settingsProvider, _jobHistoryStorage, _tempFolderProvider, _file);
        }

        [Test]
        public void HistoricJob_CanBeInitializedByJob()
        {
            var historicJob = new HistoricJob(_job);

            Assert.IsNotNull(historicJob);
        }

        [Test]
        public void Init_HistoryIsEmptyList()
        {
            Assert.IsNotNull(_jobHistoryManager.History);
            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Save_CallsJobHistoryStorageSave()
        {
            _jobHistoryManager.Save();

            _jobHistoryStorage.Received(1).Save(_jobHistoryManager.History);
        }

        [Test]
        public void Clear_HistoryGetsEmptyed_CallsJobHistoryStorageSaveWithEmptyHistory_CallsHistoryChanged()
        {
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Clear();

            Assert.IsEmpty(_jobHistoryManager.History);
            _jobHistoryStorage.Received().Save(Arg.Is<IList<HistoricJob>>(x => x.Count == 0));
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Load_CapacityFitsSavedHistory_HistoryInHistoryManagerWasLoadedFromHistoryStorage()
        {
            _settings.ApplicationSettings.JobHistory.Capacity = _storedHistory.Count;

            _jobHistoryManager.Load();
            var loadedHistory = _jobHistoryManager.History;

            Assert.AreEqual(_storedHistory, loadedHistory);
        }

        [Test]
        public void Load_CapacityIsToSmallForSavedHistory_HistoryIsReducedToCapacity()
        {
            _settings.ApplicationSettings.JobHistory.Capacity = _storedHistory.Count - 2;

            _jobHistoryManager.Load();
            var loadedHistory = _jobHistoryManager.History;

            _storedHistory.Remove(_storedHistory.Last());
            _storedHistory.Remove(_storedHistory.Last());
            Assert.AreEqual(_storedHistory, loadedHistory);
        }

        [Test]
        public void Load_RemovesDeletedJobsFromHistory()
        {
            var deletedFile = "deleted";
            _job.OutputFiles[0] = deletedFile;
            _file.Exists(deletedFile).Returns(false);
            var deletedHistoricJob = new HistoricJob(_job);
            _storedHistory.Add(deletedHistoricJob);

            _jobHistoryManager.Load(); //Adds jobs to history

            Assert.False(_jobHistoryManager.History.Contains(deletedHistoricJob), "Did not remvoe deleted file from history");
        }

        [Test]
        public void Load_CallsHistoryChanged()
        {
            _settings.ApplicationSettings.JobHistory.Capacity = _storedHistory.Count + 5;
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Load();

            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Load_CallsHistoryChanged()
        {
            _settings.ApplicationSettings.JobHistory.Capacity = _history.Count + 5;
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Load();

            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Add_HistoryDisabled_HistoryRemainsEmpty()
        {
            _settings.ApplicationSettings.JobHistory.Enabled = false;

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobIsTemporary_HistoryRemainsEmpty()
        {
            _job.OutputFilenameTemplate = Path.Combine(TempFolder, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), "file.pdf");

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobHasNoOutpufiles_HistoryRemainsEmpty()
        {
            _job.OutputFiles.Clear();

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobIsAddable_HistoryContainsJob_CallsHistoryChanged()
        {
            _job.OutputFiles.Clear();
            var filepathToIdentifyJob = "NameToIdentifyJob.pdf";
            _job.OutputFiles.Add(filepathToIdentifyJob);
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Add(_job);

            Assert.AreEqual(1, _jobHistoryManager.History.Count, "1 Job should be added.");
            Assert.AreEqual(filepathToIdentifyJob, _jobHistoryManager.History.First().Files[0], "New job is not first in history");
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Add_JobIsAddableHistoryHasFullCapacity_HistoryContainsJobCapacityIsNotExceeded_CallsHistoryChanged()
        {
            _jobHistoryManager.Load();//Adds jobs to history

            _job.OutputFiles.Clear();
            var filepathToIdentifyJob = "NameToIdentifyJob.pdf";
            _job.OutputFiles.Add(filepathToIdentifyJob);
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Add(_job);

            Assert.AreEqual(filepathToIdentifyJob, _jobHistoryManager.History.First().Files[0], "New job is not first in history");
            Assert.AreEqual(_capacity, _jobHistoryManager.History.Count, "Capacity was exceeded");
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Remove_HistoryContainsJob_JobGetsRemovedFromHistory()
        {
            _jobHistoryManager.Load(); //Adds jobs to history
            var removedJob = _jobHistoryManager.History[2];

            _jobHistoryManager.Remove(removedJob);

            Assert.AreEqual(_capacity - 1, _jobHistoryManager.History.Count);
            Assert.IsFalse(_jobHistoryManager.History.Contains(removedJob), "History contains removed job");
        }

        [Test]
        public void Remove_HistoryContainsJob_CallsHistoryChanged()
        {
            _jobHistoryManager.Load(); //Adds jobs to history
            var removedJob = _jobHistoryManager.History[2];
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Remove(removedJob);

            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Remove_HistoryDoesNotContainJob_RemoveGetsIgnored()
        {
            _jobHistoryManager.Load(); //Adds jobs to history
            var unavailableJob = new HistoricJob(_job);

            _jobHistoryManager.Remove(unavailableJob);

            Assert.AreEqual(_capacity, _jobHistoryManager.History.Count);
            Assert.IsFalse(_jobHistoryManager.History.Contains(unavailableJob));
        }

        [Test]
        public void Refresh_RemovesDeletedJobsFromHistory()
        {
            _jobHistoryManager.Load(); //Adds jobs to history
            var deletedFile = "deleted";
            _job.OutputFiles[0] = deletedFile;
            _file.Exists(deletedFile).Returns(false);
            var deletedHistoricJob = new HistoricJob(_job);
            _jobHistoryManager.History.Add(deletedHistoricJob);

            _jobHistoryManager.Refresh();

            Assert.IsFalse(_jobHistoryManager.History.Contains(deletedHistoricJob), "Did not remove deleted file from history");
        }
    }
}
