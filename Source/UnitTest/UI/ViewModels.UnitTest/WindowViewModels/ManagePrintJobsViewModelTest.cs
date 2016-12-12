using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using Rhino.Mocks;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    internal class ManagePrintJobsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            _dragDrop = new DragAndDropEventHandler(MockRepository.GenerateStub<IFileConversionHandler>());
            _jobInfoManager = MockRepository.GenerateStub<IJobInfoManager>();
        }

        private IJobInfoQueue _queueStub;
        private DragAndDropEventHandler _dragDrop;
        private IJobInfoManager _jobInfoManager;

        private ManagePrintJobsViewModel BuildViewModel()
        {
            return new ManagePrintJobsViewModel(_queueStub, _dragDrop, _jobInfoManager);
        }

        [Test]
        public void CollectionView_WithEmptyQueue_CurrentItemIsNull()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsNull(model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithMultipleJobsInQueue_CurrentItemIsFirstJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>(new[] {jobInfoStub, MockRepository.GenerateStub<JobInfo>()}));

            var model = BuildViewModel();

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithSingleJobInQueue_CurrentItemIsThisJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>(new[] {jobInfoStub}));

            var model = BuildViewModel();

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithEmptyQueue_OnJobAddedToQueue_NewJobIsCurrentItem()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            _queueStub.Raise(x => x.OnNewJobInfo += null, _queueStub, new NewJobInfoEventArgs(jobInfoStub));

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithSingleJobInQueue_OnJobAddedToQueue_ContainsTwoJobs()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>(new[] {jobInfoStub}));

            var model = BuildViewModel();

            _queueStub.Raise(x => x.OnNewJobInfo += null, _queueStub, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.AreEqual(2, model.JobInfos.Count);
        }

        [Test]
        public void CollectionViewWithSingleJobInQueue_OnJobAddedToQueue_OldJobRemainsCurrentItem()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>(new[] {jobInfoStub}));

            var model = BuildViewModel();

            _queueStub.Raise(x => x.OnNewJobInfo += null, _queueStub, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_CanExecute_IsTrue()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            var jobList = new List<JobInfo>(new[] {jobInfoStub});

            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsTrue(model.DeleteJobCommand.CanExecute(jobList));
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_CallsRemoveInJobInfoQueue()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            var jobList = new List<JobInfo>(new[] {jobInfoStub});

            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList);

            _queueStub.AssertWasCalled(x => x.Remove(jobInfoStub, true));
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_RemovesSingleJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<JobInfo>();

            var jobList = new List<JobInfo>(new[] {jobInfoStub});

            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList);

            Assert.IsTrue(model.JobInfos.Count == 0);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingFirstJob_SecondJobBecomesCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});

            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.Take(1));

            Assert.AreSame(jobList[1], model.JobInfos.CurrentItem);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingSecondJob_FirstJobBecomesCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});

            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.DeleteJobCommand.Execute(jobList.Skip(1));

            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(new List<JobInfo>()));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_ExecuteCalled_ThrowsInvalidOperationException()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.Throws<InvalidOperationException>(() => model.MergeAllJobsCommand.Execute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            _jobInfoManager.AssertWasCalled(x => x.Merge(jobList[0], jobList[1]));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_OnlyFirstJobRemainsInViewModel()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            Assert.IsFalse(model.JobInfos.Contains(jobList[1]));
            Assert.AreEqual(1, model.JobInfos.Count);
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            _queueStub.AssertWasCalled(x => x.Remove(jobList[1], false));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_SavesRemainingJobInfo()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeAllJobsCommand.Execute(null);

            _jobInfoManager.AssertWasCalled(x => x.SaveToInfFile(jobList[0]));
        }

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>();
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_CanExecuteWithSingleJobListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsTrue()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsTrue(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_WithTwoJobListParameter_FirstJobRemains()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            Assert.AreEqual(1, model.JobInfos.Count);
            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            _jobInfoManager.AssertWasCalled(x => x.Merge(jobList[0], jobList[1]));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            _queueStub.AssertWasCalled(x => x.Remove(jobList[1], false));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_SaveRemainingJobInfo()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MergeJobsCommand.Execute(jobList);

            _jobInfoManager.AssertWasCalled(x => x.SaveToInfFile(jobList[0]));
        }

        [Test]
        public void MoveDownCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>();
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveDownCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveDownCommand.CanExecute(null));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithFirstJob_IsTrue()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsTrue(model.MoveDownCommand.CanExecute(jobList.Take(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithSecondJob_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList.Skip(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_ExecutedWhileCanExecuteIsFalse_ThrowsInvalidOperationException()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.Throws<InvalidOperationException>(() => model.MoveDownCommand.Execute(jobList.Skip(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_WithFirstJob_JobRemainsCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MoveDownCommand.Execute(jobList.Take(1));

            Assert.AreEqual(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_WithFirstJob_MovesToSecondPosition()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MoveDownCommand.Execute(jobList.Take(1));

            Assert.AreEqual(1, model.JobInfos.IndexOf(jobList[0]));
        }

        [Test]
        public void MoveUpCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>();
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveUpCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            _queueStub.Stub(x => x.JobInfos).Return(new List<JobInfo>());

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveUpCommand.CanExecute(null));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithFirstJob_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList.Take(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithSecondJob_IsTrue()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsTrue(model.MoveUpCommand.CanExecute(jobList.Skip(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsFalse()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_ExecutedWhileCanExecuteIsFalse_ThrowsInvalidOperationException()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            Assert.Throws<InvalidOperationException>(() => model.MoveUpCommand.Execute(jobList.Take(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_WithSecondJob_JobRemainsCurrentItem()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MoveUpCommand.Execute(jobList.Skip(1));

            Assert.AreEqual(jobList[1], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_WithSecondJob_MovesToTopOfList()
        {
            var jobList = new List<JobInfo>(new[] {MockRepository.GenerateStub<JobInfo>(), MockRepository.GenerateStub<JobInfo>()});
            _queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = BuildViewModel();

            model.MoveUpCommand.Execute(jobList.Skip(1));

            Assert.AreEqual(0, model.JobInfos.IndexOf(jobList[1]));
        }
    }

    [TestFixture]
    public class ManagePrintJobsEventTests
    {
        [SetUp]
        public void Setup()
        {
            _jobList = new List<JobInfo>();
            _jobInfoQueue = MockRepository.GenerateStub<IJobInfoQueue>();
            _dragAndDrop = new DragAndDropEventHandler(MockRepository.GenerateStub<IFileConversionHandler>());
            _jobInfoManager = MockRepository.GenerateStub<IJobInfoManager>();
            _jobInfoQueue.Stub(x => x.JobInfos).Return(_jobList);
            _eventWasRaised = false;

            _model = new ManagePrintJobsViewModel(_jobInfoQueue, _dragAndDrop, _jobInfoManager);
        }

        private IJobInfoQueue _jobInfoQueue;
        private IList<JobInfo> _jobList;
        private bool _eventWasRaised;
        private ManagePrintJobsViewModel _model;
        private DragAndDropEventHandler _dragAndDrop;
        private IJobInfoManager _jobInfoManager;

        [Test]
        public void DeleteJobCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_CurrentItemChanges_CommandRaisesCanExecuteChanged()
        {
            _model = new ManagePrintJobsViewModel(_jobInfoQueue, _dragAndDrop, _jobInfoManager);
            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));
            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.JobInfos.MoveCurrentToNext();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeAllJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeAllJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MoveUpCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MoveUpCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<JobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesDeleteJobCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMergeAllJobsCanExecuteChanged()
        {
            _model.MergeAllJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMergeJobsCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMoveDownCanExecuteChanged()
        {
            _model.MoveDownCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMoveUpCanExecuteChanged()
        {
            _model.MoveUpCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }
    }
}