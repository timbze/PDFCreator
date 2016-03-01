using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.ViewModels;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels
{
    [TestFixture]
    class ManagePrintJobsViewModelTest
    {
        
        #region CollectionView
        
        [Test]
        public void CollectionView_WithEmptyQueue_CurrentItemIsNull()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsNull(model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithSingleJobInQueue_CurrentItemIsThisJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>(new[] { jobInfoStub }));

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionView_WithMultipleJobsInQueue_CurrentItemIsFirstJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>(new[] { jobInfoStub, MockRepository.GenerateStub<IJobInfo>() }));

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithEmptyQueue_OnJobAddedToQueue_NewJobIsCurrentItem()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();

            queueStub.Raise(x => x.OnNewJobInfo += null, queueStub, new NewJobInfoEventArgs(jobInfoStub));

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        [Test]
        public void CollectionViewWithSingleJobInQueue_OnJobAddedToQueue_ContainsTwoJobs()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>(new[] { jobInfoStub }));

            var model = new ManagePrintJobsViewModel(queueStub);

            queueStub.Raise(x => x.OnNewJobInfo += null, queueStub, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.AreEqual(2, model.JobInfos.Count);
        }

        [Test]
        public void CollectionViewWithSingleJobInQueue_OnJobAddedToQueue_OldJobRemainsCurrentItem()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>(new[] { jobInfoStub }));

            var model = new ManagePrintJobsViewModel(queueStub);

            queueStub.Raise(x => x.OnNewJobInfo += null, queueStub, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.AreSame(jobInfoStub, model.JobInfos.CurrentItem);
        }

        #endregion

        #region DeleteJobCommand

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_CanExecute_IsTrue()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();

            var jobList = new List<IJobInfo>(new[] { jobInfoStub });

            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsTrue(model.DeleteJobCommand.CanExecute(jobList));
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_RemovesSingleJob()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();

            var jobList = new List<IJobInfo>(new[] { jobInfoStub });

            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.DeleteJobCommand.Execute(jobList);

            Assert.IsTrue(model.JobInfos.Count == 0);
        }

        [Test]
        public void DeleteJobCommandWithSingleJobInQueue_Execute_CallsRemoveInJobInfoQueue()
        {
            var jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();

            var jobList = new List<IJobInfo>(new[] { jobInfoStub });

            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.DeleteJobCommand.Execute(jobList);

            queueStub.AssertWasCalled(x => x.Remove(jobInfoStub, true));
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingFirstJob_SecondJobBecomesCurrentItem()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });

            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.DeleteJobCommand.Execute(jobList.Take(1));

            Assert.AreSame(jobList[1], model.JobInfos.CurrentItem);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_DeletingSecondJob_FirstJobBecomesCurrentItem()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });

            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.DeleteJobCommand.Execute(jobList.Skip(1));

            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        #endregion

        #region MergeAllJobsCommand

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MergeAllJobsCommand.CanExecute(new List<IJobInfo>()));
        }

        [Test]
        public void MergeAllJobsCommandWithEmptyQueue_ExecuteCalled_ThrowsInvalidOperationException()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.Throws<InvalidOperationException>(() => model.MergeAllJobsCommand.Execute(null));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeAllJobsCommand.Execute(null);

            jobList[0].AssertWasCalled(x => x.Merge(jobList[1]));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_OnlyFirstJobRemainsInViewModel()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeAllJobsCommand.Execute(null);

            Assert.IsFalse(model.JobInfos.Contains(jobList[1]));
            Assert.AreEqual(1, model.JobInfos.Count);
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeAllJobsCommand.Execute(null);

            queueStub.AssertWasCalled(x => x.Remove(jobList[1], false));
        }

        [Test]
        public void MergeAllJobsCommandWithTwoJobsInQueue_ExecuteCalled_SavesRemainingJobInfo()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeAllJobsCommand.Execute(null);

            jobList[0].AssertWasCalled(x => x.SaveInf());
        }

        #endregion

        #region MergeJobsCommand

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(null));
        }

        [Test]
        public void MergeJobsCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>();
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_CanExecuteWithSingleJobListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsTrue()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsTrue(model.MergeJobsCommand.CanExecute(jobList));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobQueue_WithTwoJobListParameter_FirstJobRemains()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);
            
            model.MergeJobsCommand.Execute(jobList);

            Assert.AreEqual(1, model.JobInfos.Count);
            Assert.AreSame(jobList[0], model.JobInfos.CurrentItem);
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_CallsMergeOnFirstJobWithSecondJobAsParam()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeJobsCommand.Execute(jobList);

            jobList[0].AssertWasCalled(x => x.Merge(jobList[1]));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_RemovesJobInfoFromQueue()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeJobsCommand.Execute(jobList);

            queueStub.AssertWasCalled(x => x.Remove(jobList[1], false));
        }

        [Test]
        public void MergeJobsCommandWithTwoJobsInQueue_ExecuteCalled_SaveRemainingJobInfo()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MergeJobsCommand.Execute(jobList);

            jobList[0].AssertWasCalled(x => x.SaveInf());
        }

        #endregion

        #region MoveUpCommand

        [Test]
        public void MoveUpCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveUpCommand.CanExecute(null));
        }

        [Test]
        public void MoveUpCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>();
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithFirstJob_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveUpCommand.CanExecute(jobList.Take(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_CanExecuteWithSecondJob_IsTrue()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsTrue(model.MoveUpCommand.CanExecute(jobList.Skip(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_ExecutedWhileCanExecuteIsFalse_ThrowsInvalidOperationException()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.Throws<InvalidOperationException>(() => model.MoveUpCommand.Execute(jobList.Take(1)));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_WithSecondJob_MovesToTopOfList()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MoveUpCommand.Execute(jobList.Skip(1));

            Assert.AreEqual(0, model.JobInfos.IndexOf(jobList[1]));
        }

        [Test]
        public void MoveUpCommandWithTwoJobQueue_WithSecondJob_JobRemainsCurrentItem()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MoveUpCommand.Execute(jobList.Skip(1));

            Assert.AreEqual(jobList[1], model.JobInfos.CurrentItem);
        }

        #endregion

        #region MoveDownCommand

        [Test]
        public void MoveDownCommandWithEmptyQueue_CanExecuteWithNullParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            queueStub.Stub(x => x.JobInfos).Return(new List<IJobInfo>());

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveDownCommand.CanExecute(null));
        }

        [Test]
        public void MoveDownCommandWithEmptyQueue_CanExecuteWithEmptyListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>();
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithTwoJobListParameter_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithFirstJob_IsTrue()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsTrue(model.MoveDownCommand.CanExecute(jobList.Take(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_CanExecuteWithSecondJob_IsFalse()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.IsFalse(model.MoveDownCommand.CanExecute(jobList.Skip(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_ExecutedWhileCanExecuteIsFalse_ThrowsInvalidOperationException()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            Assert.Throws<InvalidOperationException>(() => model.MoveDownCommand.Execute(jobList.Skip(1)));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_WithFirstJob_MovesToSecondPosition()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MoveDownCommand.Execute(jobList.Take(1));

            Assert.AreEqual(1, model.JobInfos.IndexOf(jobList[0]));
        }

        [Test]
        public void MoveDownCommandWithTwoJobQueue_WithFirstJob_JobRemainsCurrentItem()
        {
            var queueStub = MockRepository.GenerateStub<IJobInfoQueue>();
            var jobList = new List<IJobInfo>(new[] { MockRepository.GenerateStub<IJobInfo>(), MockRepository.GenerateStub<IJobInfo>() });
            queueStub.Stub(x => x.JobInfos).Return(jobList);

            var model = new ManagePrintJobsViewModel(queueStub);

            model.MoveDownCommand.Execute(jobList.Take(1));

            Assert.AreEqual(jobList[0], model.JobInfos.CurrentItem);
        }

        #endregion
    }

    [TestFixture]
    public class ManagePrintJobsEventTests
    {
        private IJobInfoQueue _jobInfoQueue;
        private IList<IJobInfo> _jobList; 
        private bool _eventWasRaised;
        private ManagePrintJobsViewModel _model;

        [SetUp]
        public void Setup()
        {
            _jobList = new List<IJobInfo>();
            _jobInfoQueue = MockRepository.GenerateStub<IJobInfoQueue>();
            _jobInfoQueue.Stub(x => x.JobInfos).Return(_jobList);
            _eventWasRaised = false;

            _model = new ManagePrintJobsViewModel(_jobInfoQueue);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_CurrentItemChanges_CommandRaisesCanExecuteChanged()
        {
            _model = new ManagePrintJobsViewModel(_jobInfoQueue);
            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));
            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.JobInfos.MoveCurrentToNext();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void DeleteJobCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeAllJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeAllJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MoveUpCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MoveUpCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.Raise(x => x.OnNewJobInfo += null, _jobInfoQueue, new NewJobInfoEventArgs(MockRepository.GenerateStub<IJobInfo>()));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMoveUpCanExecuteChanged()
        {
            _model.MoveUpCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

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
        public void ViewModel_RaiseRefreshView_RaisesMergeJobsCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

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
        public void ViewModel_RaiseRefreshView_RaisesDeleteJobCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }
    }
}
