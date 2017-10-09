using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.UnitTest.Windows
{
    [TestFixture]
    internal class ManagePrintJobsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _wasCalled = false;
            _queueStub = Substitute.For<IJobInfoQueue>();
            _dragDrop = new DragAndDropEventHandler(Substitute.For<IFileConversionHandler>());
            _jobInfoManager = Substitute.For<IJobInfoManager>();
            _dispatcher = new InvokeImmediatelyDispatcher();
            _jobInfoStub = new JobInfo();
        }

        private IJobInfoQueue _queueStub;
        private JobInfo _jobInfoStub;
        private DragAndDropEventHandler _dragDrop;
        private IJobInfoManager _jobInfoManager;
        private IDispatcher _dispatcher;
        private bool _wasCalled;

        private ManagePrintJobsViewModel BuildViewModel()
        {
            return new ManagePrintJobsViewModel(_queueStub, _dragDrop, _jobInfoManager, _dispatcher, new DesignTimeTranslationUpdater());
        }

        [Test]
        public void CollectionView_WithEmptyQueue_CurrentItemIsNull()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsNull(model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithMultipleJobsInQueue_CurrentItemIsFirstJob()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo> { _jobInfoStub });

            var model = BuildViewModel();

            Assert.AreSame(_jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithSingleJobInQueue_CurrentItemIsThisJob()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>() { _jobInfoStub });

            var model = BuildViewModel();

            Assert.AreSame(_jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithEmptyQueue_OnJobAddedToQueue_NewJobIsCurrentItem()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo> { _jobInfoStub });

            var model = BuildViewModel();

            _queueStub.OnNewJobInfo += (sender, args) => _wasCalled = true;
            _queueStub.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));

            Assert.IsTrue(_wasCalled);
            Assert.AreSame(_jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithSingleJobInQueue_OnJobAddedToQueue_OldJobRemainsCurrentItem()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>() { _jobInfoStub });
            var model = BuildViewModel();

            _queueStub.OnNewJobInfo += (sender, args) => _wasCalled = true;
            _queueStub.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));
            Assert.IsTrue(_wasCalled);
            Assert.AreSame(_jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_CallsRemoveInJobInfoQueue()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);
            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.First());

            _queueStub.Received().Remove(_jobInfoStub, true);
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_RemovesSingleJob()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub });

            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.First());

            Assert.IsTrue(model.JobInfos.Count == 0);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingFirstJob_SecondJobBecomesCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, new JobInfo(), });
            var secondJob = jobList[1];
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.First());

            Assert.AreSame(secondJob, model.JobInfos.CurrentItem);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingSecondJob_FirstJobBecomesCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, new JobInfo(), });

            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.Skip(1));

            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>());
            var model = BuildViewModel();
            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(new List<JobInfo>()));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>());
            var model = BuildViewModel();
            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_ExecuteCalled_ThrowsInvalidOperationException()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>());
            var model = BuildViewModel();
            Assert.Throws<InvalidOperationException>(() => model.MergeAllJobsCommand.Execute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, new JobInfo(), });
            var firstJob = jobList[0];
            var secondJob = jobList[1];
            _queueStub.JobInfos.Returns(jobList);
            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            _jobInfoManager.Received().Merge(firstJob, secondJob);
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_OnlyFirstJobRemainsInViewModel()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, new JobInfo(), });
            var secondJob = jobList[1];
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            Assert.IsFalse(model.JobInfos.Contains(secondJob));
            Assert.AreEqual(1, model.JobInfos.Count);
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, new JobInfo(), });
            var secondJob = jobList[1];
            _queueStub.JobInfos.Returns(jobList);
            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            _queueStub.Received().Remove(secondJob, false);
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_SavesRemainingJobInfo()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo> { _jobInfoStub, new JobInfo() });
            var viewModel = BuildViewModel();

            viewModel.MergeAllJobsCommand.Execute(null);

            _jobInfoManager.Received().SaveToInfFile(_jobInfoStub);
        }

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>();
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.JobInfos.Returns(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_CanExecuteWithSingleJobListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsTrue()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            Assert.IsTrue(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_WithTwoJobListParameter_FirstJobRemains()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            Assert.AreEqual(1, model.JobInfos.Count);
            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);
            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);
            _jobInfoManager.Received().Merge(_jobInfoStub, _jobInfoStub);
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            _queueStub.Received().Remove(jobList[0], false);
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_SaveRemainingJobInfo()
        {
            var jobList = new List<JobInfo>(new[] { _jobInfoStub, _jobInfoStub });
            _queueStub.JobInfos.Returns(jobList);
            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            _jobInfoManager.Received().SaveToInfFile(jobList[0]);
        }
    }
}
