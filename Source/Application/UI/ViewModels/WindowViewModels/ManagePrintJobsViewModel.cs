using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class ManagePrintJobsViewModel : InteractionAwareViewModelBase<ManagePrintJobsInteraction>
    {
        private readonly DragAndDropEventHandler _dragAndDrop;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IDispatcher _dispatcher;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly ObservableCollection<JobInfo> _jobInfos;
        private ManagePrintJobsWindowTranslation _translation;

        public ManagePrintJobsViewModel(IJobInfoQueue jobInfoQueue, DragAndDropEventHandler dragAndDrop, IJobInfoManager jobInfoManager, IDispatcher dispatcher, ManagePrintJobsWindowTranslation translation)
        {
            _jobInfoQueue = jobInfoQueue;
            _dragAndDrop = dragAndDrop;
            _jobInfoManager = jobInfoManager;
            _dispatcher = dispatcher;
            _translation = translation;
            _jobInfoQueue.OnNewJobInfo += OnNewJobInfo;

            ListSelectionChangedCommand = new DelegateCommand(ListSelectionChanged);
            DeleteJobCommand = new DelegateCommand(ExecuteDeleteJob, CanExecuteDeleteJob);
            MergeJobsCommand = new DelegateCommand(ExecuteMergeJobs, CanExecuteMergeJobs);
            MergeAllJobsCommand = new DelegateCommand(ExecuteMergeAllJobs, CanExecuteMergeAllJobs);
            MoveUpCommand = new DelegateCommand(ExecuteMoveUp, CanExecuteMoveUp);
            MoveDownCommand = new DelegateCommand(ExecuteMoveDown, CanExecuteMoveDown);
            WindowClosedCommand = new DelegateCommand(OnWindowClosed);
            WindowActivatedCommand = new DelegateCommand(OnWindowActivated);
            DragEnterCommand = new DelegateCommand<DragEventArgs>(OnDragEnter);
            DropCommand = new DelegateCommand<DragEventArgs>(OnDrop);
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(OnKeyDown);

            _jobInfos = new ObservableCollection<JobInfo>();
            JobInfos = new CollectionView(_jobInfos);
            JobListSelectionChanged = new DelegateCommand(ListItemCHange);
            foreach (var jobInfo in _jobInfoQueue.JobInfos)
            {
                AddJobInfo(jobInfo);
            }
        }

        private void ListItemCHange(object obj)
        {
            RaiseRefreshView();
        }

        public ManagePrintJobsWindowTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
        }

        public CollectionView JobInfos { get; }
        public DelegateCommand ListSelectionChangedCommand { get; }
        public DelegateCommand DeleteJobCommand { get; }
        public DelegateCommand MergeJobsCommand { get; }
        public DelegateCommand MergeAllJobsCommand { get; }
        public DelegateCommand MoveUpCommand { get; }
        public DelegateCommand MoveDownCommand { get; }
        public DelegateCommand WindowClosedCommand { get; }
        public DelegateCommand WindowActivatedCommand { get; }
        public DelegateCommand<DragEventArgs> DragEnterCommand { get; }
        public DelegateCommand<DragEventArgs> DropCommand { get; }
        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; }

        public DelegateCommand JobListSelectionChanged { get; set; }

        private void ListSelectionChanged(object obj)
        {
            DeleteJobCommand.RaiseCanExecuteChanged();
            MergeJobsCommand.RaiseCanExecuteChanged();
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                FinishInteraction();
        }

        private void OnWindowActivated(object obj)
        {
            RaiseRefreshView();
        }

        private void OnDrop(DragEventArgs e)
        {
            _dragAndDrop.HandleDropEvent(e);
        }

        private void OnDragEnter(DragEventArgs e)
        {
            _dragAndDrop.HandleDragEnter(e);
        }

        private void OnWindowClosed(object obj)
        {
            _jobInfoQueue.OnNewJobInfo -= OnNewJobInfo;
        }

        private void OnNewJobInfo(object sender, NewJobInfoEventArgs e)
        {
            Action<JobInfo> addMethod = AddJobInfo;
            _dispatcher.BeginInvoke(addMethod, e.JobInfo);
        }

        private void AddJobInfo(JobInfo jobInfo)
        {
            _jobInfos.Add(jobInfo);

            if (JobInfos.CurrentItem == null)
                JobInfos.MoveCurrentToFirst();

            RaiseRefreshView();
        }

        public void RaiseRefreshView()
        {
            RaiseRefreshView(false);
        }

        private void RaiseRefreshView(bool refreshCollectionView)
        {
            DeleteJobCommand.RaiseCanExecuteChanged();
            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();

            if (refreshCollectionView)
                JobInfos.Refresh();
        }

        private void ExecuteDeleteJob(object o)
        {
            var jobs = o as IEnumerable<object>;
            if (jobs == null)
                return;

            foreach (var job in jobs.ToArray())
            {
                var jobInfo = (JobInfo) job;
                var position = JobInfos.CurrentPosition;
                _jobInfos.Remove(jobInfo);
                _jobInfoQueue.Remove(jobInfo, true);

                if (_jobInfos.Count > 0)
                    JobInfos.MoveCurrentToPosition(Math.Max(0, position - 1));
            }

            RaiseRefreshView();
        }

        private bool CanExecuteDeleteJob(object o)
        {
            var jobs = o as IEnumerable<object>;
            return jobs != null && jobs.Any();
        }

        private void ExecuteMergeJobs(object o)
        {
            if (!CanExecuteMergeJobs(o))
                throw new InvalidOperationException("CanExecute is false");

            var jobObjects = o as IEnumerable<object>;
            if (jobObjects == null)
                return;

            var jobs = jobObjects.ToList();
            var first = (JobInfo) jobs.First();

            foreach (var jobObject in jobs.Skip(1))
            {
                var job = (JobInfo) jobObject;
                if (job.JobType != first.JobType)
                    continue;

                _jobInfoManager.Merge(first, job);
                _jobInfos.Remove(job);
                _jobInfoQueue.Remove(job, false);
            }

            _jobInfoManager.SaveToInfFile(first);

            RaiseRefreshView(true);
        }

        private bool CanExecuteMergeJobs(object o)
        {
            var jobs = o as IEnumerable<object>;
            return jobs != null && jobs.Count() > 1;
        }

        private void ExecuteMergeAllJobs(object o)
        {
            ExecuteMergeJobs(_jobInfos);
        }

        private bool CanExecuteMergeAllJobs(object o)
        {
            return _jobInfos.Count > 1;
        }

        private void ExecuteMoveUp(object o)
        {
            if (!CanExecuteMoveUp(o))
                throw new InvalidOperationException();

            var jobs = o as IEnumerable<object>;

            if (jobs == null)
                return;

            var job = (JobInfo) jobs.First();

            MoveJob(job, -1);
            RaiseRefreshView();
        }

        private bool CanExecuteMoveUp(object o)
        {
            var selectedJobs = (o as IEnumerable<object>)?.ToList();
            if (selectedJobs == null)
            {
                return false;
            }

            if (selectedJobs.Count != 1)
            {
                return false;
            }

            var indexOfSelectedElement = _jobInfos.IndexOf((JobInfo)selectedJobs.First());
            // if indexofselected element is not on the bottom of list we can move up (meaning it is not first element in list)
            return indexOfSelectedElement > 0;
        }

        private void ExecuteMoveDown(object o)
        {
            if (!CanExecuteMoveDown(o))
                throw new InvalidOperationException();

            var jobs = o as IEnumerable<object>;

            if (jobs == null)
                return;

            var job = (JobInfo) jobs.First();

            MoveJob(job, +1);
            RaiseRefreshView();
        }

        private bool CanExecuteMoveDown(object o)
        {
            var selectedJobs = (o as IEnumerable<object>)?.ToList();
            if (selectedJobs == null)
            {
                return false;
            }

            if (selectedJobs.Count != 1)
            {
                return false;
            }

            var indexOfSelectedElement = _jobInfos.IndexOf((JobInfo)selectedJobs.First());
            // if indexofselected element is not on the bottom of list we can move up
            return indexOfSelectedElement < _jobInfos.Count -1;
        }

        private void MoveJob(JobInfo jobInfo, int positionDifference)
        {
            var oldIndex = _jobInfos.IndexOf(jobInfo);
            _jobInfos.Move(oldIndex, oldIndex + positionDifference);

            JobInfos.MoveCurrentToPosition(oldIndex + positionDifference);
        }
    }
}