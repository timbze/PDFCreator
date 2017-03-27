using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using Rhino.Mocks;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    public class PrintJobViewModelTests
    {
        private PrintJobViewModel CreateSomePrintJobViewModel(string applicationName = "PDFCreator")
        {
            return CreateSomePrintJobViewModelWithQueue(MockRepository.GenerateStub<IJobInfoQueue>(), applicationName);
        }

        private PrintJobViewModel CreateSomePrintJobViewModelWithJobInfo(JobInfo jobInfo)
        {
            return CreateMockedPrintJobViewModel(MockRepository.GenerateStub<IJobInfoQueue>(), jobInfo);
        }

        private PrintJobViewModel CreateSomePrintJobViewModelWithQueue(IJobInfoQueue jobInfoQueue, string applicationName = "PDFCreator")
        {
            var jobInfo = new JobInfo
            {
                Metadata = new Metadata()
            };

            return CreateMockedPrintJobViewModel(jobInfoQueue, jobInfo, applicationName);
        }

        private PrintJobViewModel CreateMockedPrintJobViewModel(IJobInfoQueue jobInfoQueue, JobInfo jobInfo, string applicationName = "PDFCreator")
        {
            var appSettings = new ApplicationSettings();
            var profiles = new List<ConversionProfile>();

            var selectedProfile = new ConversionProfile();
            profiles.Add(selectedProfile);

            var interaction = new PrintJobInteraction(jobInfo, selectedProfile);

            var settings = new PdfCreatorSettings(MockRepository.GenerateStub<IStorage>());
            settings.ApplicationSettings = appSettings;
            settings.ConversionProfiles = profiles;

            var settingsHelper = Substitute.For<ISettingsProvider>();
            settingsHelper.Settings.Returns(settings);

            var settingsManager = Substitute.For<ISettingsManager>();
            settingsManager.GetSettingsProvider().Returns(settingsHelper);

            var translator = new TranslationFactory();

            var userGuideHelper = Substitute.For<IUserGuideHelper>();

            var printJobViewModel = new PrintJobViewModel(settingsManager, jobInfoQueue, new PrintJobViewModelTranslation(), new DragAndDropEventHandler(MockRepository.GenerateStub<IFileConversionHandler>()), MockRepository.GenerateStub<IInteractionInvoker>(), userGuideHelper, new ApplicationNameProvider(applicationName), new InvokeImmediatelyDispatcher());
            printJobViewModel.SetInteraction(interaction);
            printJobViewModel.FinishInteraction = () => { };

            return printJobViewModel;
        }

        private PrintJobViewModel BuildViewModel(ApplicationSettings appSettings, IList<ConversionProfile> profiles, IJobInfoQueue queue, ConversionProfile preselectedProfile = null)
        {
            var settings = new PdfCreatorSettings(MockRepository.GenerateStub<IStorage>());
            settings.ApplicationSettings = appSettings;
            settings.ConversionProfiles = profiles;

            var settingsHelper = Substitute.For<ISettingsProvider>();
            settingsHelper.Settings.Returns(settings);

            var settingsManager = Substitute.For<ISettingsManager>();
            settingsManager.GetSettingsProvider().Returns(settingsHelper);

            var userGuideHelper = Substitute.For<IUserGuideHelper>();

            var printJobViewModel = new PrintJobViewModel(settingsManager, queue, new PrintJobViewModelTranslation(), new DragAndDropEventHandler(Substitute.For<IFileConversionHandler>()), MockRepository.GenerateStub<IInteractionInvoker>(), userGuideHelper, new ApplicationNameProvider("PDFCreator"), new InvokeImmediatelyDispatcher());

            var interaction = new PrintJobInteraction(null, preselectedProfile);

            var interactionHelper = new InteractionHelper<PrintJobInteraction>(printJobViewModel, interaction);

            return printJobViewModel;
        }

        [Test]
        public void JobInfo_SaveMetadata_ContainsChangedMetadata()
        {
            var metadata = new Metadata
            {
                Title = "Title",
                Author = "Author",
                Subject = "Subject",
                Keywords = "Keywords"
            };

            var jobInfo = MockRepository.GenerateStub<JobInfo>();
            jobInfo.Metadata = metadata.Copy();

            var printJobViewModel = CreateSomePrintJobViewModelWithJobInfo(jobInfo);

            printJobViewModel.Metadata.Title = "MyTitle";
            printJobViewModel.Metadata.Author = "MyAuthor";
            printJobViewModel.Metadata.Subject = "MySubject";
            printJobViewModel.Metadata.Keywords = "MyKeywords";

            printJobViewModel.SaveCommand.Execute(null);

            Assert.AreEqual("MyTitle", printJobViewModel.JobInfo.Metadata.Title);
            Assert.AreEqual("MyAuthor", printJobViewModel.JobInfo.Metadata.Author);
            Assert.AreEqual("MySubject", printJobViewModel.JobInfo.Metadata.Subject);
            Assert.AreEqual("MyKeywords", printJobViewModel.JobInfo.Metadata.Keywords);
        }

        [Test]
        public void JobInfo_SaveMetadataWithoutChanges_ContainsSameMetadata()
        {
            var metadata = new Metadata
            {
                Title = "Title",
                Author = "Author",
                Subject = "Subject",
                Keywords = "Keywords"
            };

            var jobInfo = MockRepository.GenerateStub<JobInfo>();
            jobInfo.Metadata = metadata.Copy();

            var printJobViewModel = CreateSomePrintJobViewModelWithJobInfo(jobInfo);

            printJobViewModel.SaveCommand.Execute(null);

            Assert.AreEqual(metadata.Title, printJobViewModel.JobInfo.Metadata.Title);
            Assert.AreEqual(metadata.Author, printJobViewModel.JobInfo.Metadata.Author);
            Assert.AreEqual(metadata.Subject, printJobViewModel.JobInfo.Metadata.Subject);
            Assert.AreEqual(metadata.Keywords, printJobViewModel.JobInfo.Metadata.Keywords);
        }

        [Test]
        public void ViewModel_WithEmailCommand_HasEmailAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.EmailCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.EMail, printJobViewModel.Interaction.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithEmptyQueue_DoesAllowManagePrintJobs()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(0);

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.IsTrue(printJobViewModel.ManagePrintJobsCommand.IsExecutable);
        }

        [Test]
        public void ViewModel_WithManagePrintJobsCommand_HasSaveAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.ManagePrintJobsCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.ManagePrintJobs, printJobViewModel.Interaction.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithoutChanges_HasCancelAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            Assert.AreEqual(PrintJobAction.Cancel, printJobViewModel.Interaction.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithSaveCommand_HasSaveAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.SaveCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.Save, printJobViewModel.Interaction.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithSingleJobQueue_DoesAllowManagePrintJobs()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(1);

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.IsTrue(printJobViewModel.ManagePrintJobsCommand.IsExecutable);
        }

        [Test]
        public void ViewModel_WithTwoJobQueue_DoesAllowManagePrintJobs()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(2);

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.IsTrue(printJobViewModel.ManagePrintJobsCommand.IsExecutable);
        }

        [Test]
        public void ViewModelWithOneJob_PendingJobsText_IsNull()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(1);

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.AreEqual("Print more documents to merge or rearrange them", printJobViewModel.PendingJobsText);
        }

        [Test]
        public void ViewModelWithOneJobs_PendingJobsText_IsCorrect()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(2);
            var translation = new PrintJobViewModelTranslation();

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.AreEqual(translation.FormatMoreJobsWaiting(1), printJobViewModel.PendingJobsText);
        }

        [Test]
        public void ViewModelWithProfiles_AndAppSettings_InitializedWithLastUsedProfile()
        {
            var appSettings = new ApplicationSettings();

            const string guid = "guid2";

            appSettings.LastUsedProfileGuid = guid;

            IList<ConversionProfile> profiles = new List<ConversionProfile>();
            profiles.Add(new ConversionProfile {Guid = "guid1"});
            profiles.Add(new ConversionProfile {Guid = guid});

            var printJobViewModel = BuildViewModel(appSettings, profiles,
                MockRepository.GenerateStub<IJobInfoQueue>());

            Assert.AreEqual(guid, printJobViewModel.SelectedProfile.Guid);
        }

        [Test]
        public void ViewModelWithProfiles_PreSelectedProfileIsSelected()
        {
            var appSettings = new ApplicationSettings();

            appSettings.LastUsedProfileGuid = "guid1";

            IList<ConversionProfile> profiles = new List<ConversionProfile>();

            var preselectedConversionProfile = new ConversionProfile {Guid = "Preselected"};

            profiles.Add(new ConversionProfile {Guid = "guid1"});
            profiles.Add(preselectedConversionProfile);

            var printJobViewModel = BuildViewModel(appSettings, profiles,
                MockRepository.GenerateStub<IJobInfoQueue>(), preselectedConversionProfile);

            Assert.AreEqual(preselectedConversionProfile.Guid, printJobViewModel.SelectedProfile.Guid,
                "PreselectedProfile not selected in profile combo box");
        }

        [Test]
        public void ViewModelWithProfiles_SelectProfiles_SelectsCorrectProfile()
        {
            var appSettings = new ApplicationSettings();

            appSettings.LastUsedProfileGuid = "guid1";

            IList<ConversionProfile> profiles = new List<ConversionProfile>();
            profiles.Add(new ConversionProfile {Guid = "guid1"});
            profiles.Add(new ConversionProfile {Guid = "guid2"});

            var printJobViewModel = BuildViewModel(appSettings, profiles,
                MockRepository.GenerateStub<IJobInfoQueue>());

            const string guid = "guid2";

            printJobViewModel.SelectProfileByGuid(guid);

            Assert.AreEqual(guid, printJobViewModel.SelectedProfile.Guid);
        }

        [Test]
        public void ViewModelWithThreeJobs_PendingJobsText_IsCorrect()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.Count).Return(3);

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.AreEqual("There are 2 more Jobs waiting", printJobViewModel.PendingJobsText);
        }

        [Test]
        public void ViewModel_Title_IsEditionName()
        {
            var title = "My PDFCreator!";

            var viewModel = CreateSomePrintJobViewModel(title);

            Assert.AreEqual(title, viewModel.Title);
        }
    }
}