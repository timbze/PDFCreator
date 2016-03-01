using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using pdfforge.PDFCreator;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.ViewModels;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels
{
    [TestFixture]
    public class PrintJobViewModelTests
    {
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

            var printJobViewModel = CreateSomePrintJobViewModelWithQueue(queueStub);

            Assert.AreEqual("One more Job waiting", printJobViewModel.PendingJobsText);
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
        public void EmptyViewModel_OnNewJob_CalledRaisePropertyChanged()
        {
            var profiles = new List<ConversionProfile>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var printJobViewModel = new PrintJobViewModel(new ApplicationSettings(), profiles, queueStub);
            printJobViewModel.PropertyChanged += eventStub.OnEventRaised;
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var jobInfoEventArgs = new NewJobInfoEventArgs(jobInfoStub);
            var propertyListener = new PropertyChangedListenerMock(printJobViewModel, "JobCount");

            queueStub.Raise(x => x.OnNewJobInfo += null, jobInfoStub, jobInfoEventArgs);

            Assert.IsTrue(propertyListener.WasCalled);
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

            var jobInfo = MockRepository.GenerateStub<IJobInfo>();
            jobInfo.Metadata = metadata.Copy();

            var printJobViewModel = CreateSomePrintJobViewModelWithJobInfo(jobInfo);

            printJobViewModel.SaveCommand.Execute(null);

            Assert.AreEqual(metadata.Title, printJobViewModel.JobInfo.Metadata.Title);
            Assert.AreEqual(metadata.Author, printJobViewModel.JobInfo.Metadata.Author);
            Assert.AreEqual(metadata.Subject, printJobViewModel.JobInfo.Metadata.Subject);
            Assert.AreEqual(metadata.Keywords, printJobViewModel.JobInfo.Metadata.Keywords);
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

            var jobInfo = MockRepository.GenerateStub<IJobInfo>();
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
        public void ViewModelWithProfiles_SelectProfiles_SelectsCorrectProfile()
        {
            var appSettings = new ApplicationSettings();

            appSettings.LastUsedProfileGuid = "guid1";

            IList<ConversionProfile> profiles = new List<ConversionProfile>();
            profiles.Add(new ConversionProfile {Guid = "guid1"});
            profiles.Add(new ConversionProfile {Guid = "guid2"});

            var printJobViewModel = new PrintJobViewModel(appSettings, profiles, MockRepository.GenerateStub<IJobInfoQueue>());

            const string guid = "guid2";

            printJobViewModel.SelectProfileByGuid(guid);

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

            var printJobViewModel = new PrintJobViewModel(appSettings, profiles, MockRepository.GenerateStub<IJobInfoQueue>(), preselectedConversionProfile);

            Assert.AreEqual(preselectedConversionProfile.Guid, printJobViewModel.SelectedProfile.Guid, "PreselectedProfile not selected in profile combo box");
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

            var printJobViewModel = new PrintJobViewModel(appSettings, profiles, MockRepository.GenerateStub<IJobInfoQueue>());

            Assert.AreEqual(guid, printJobViewModel.SelectedProfile.Guid);
        }

        [Test]
        public void ViewModel_WithoutChanges_HasCancelAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            Assert.AreEqual(PrintJobAction.Cancel, printJobViewModel.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithSaveCommand_HasSaveAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.SaveCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.Save, printJobViewModel.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithEmailCommand_HasEmailAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.EmailCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.EMail, printJobViewModel.PrintJobAction);
        }

        [Test]
        public void ViewModel_WithManagePrintJobsCommand_HasSaveAction()
        {
            var printJobViewModel = CreateSomePrintJobViewModel();

            printJobViewModel.ManagePrintJobsCommand.Execute(null);

            Assert.AreEqual(PrintJobAction.ManagePrintJobs, printJobViewModel.PrintJobAction);
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

        private PrintJobViewModel CreateSomePrintJobViewModel()
        {
            return CreateSomePrintJobViewModelWithQueue(MockRepository.GenerateStub<IJobInfoQueue>());
        }

        private PrintJobViewModel CreateSomePrintJobViewModelWithJobInfo(IJobInfo jobInfo)
        {
            return CreateMockedPrintJobViewModel(MockRepository.GenerateStub<IJobInfoQueue>(), jobInfo);
        }

        private PrintJobViewModel CreateSomePrintJobViewModelWithQueue(IJobInfoQueue jobInfoQueue)
        {
            var jobInfo = new JobInfo
            {
                Metadata = new Metadata()
            };

            return CreateMockedPrintJobViewModel(jobInfoQueue, jobInfo);
        }

        private PrintJobViewModel CreateMockedPrintJobViewModel(IJobInfoQueue jobInfoQueue, IJobInfo jobInfo)
        {
            var appSettings = new ApplicationSettings();
            var profiles = new List<ConversionProfile>();

            var selectedProfile = new ConversionProfile();
            profiles.Add(selectedProfile);

            var translationHelper = new TranslationHelper();
            translationHelper.InitEmptyTranslator();

            return new PrintJobViewModel(appSettings, profiles, jobInfoQueue, selectedProfile, jobInfo, translationHelper);
        }
    }
}