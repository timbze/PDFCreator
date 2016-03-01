using System;
using System.Runtime.InteropServices;
using System.Threading;
using NLog;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Helper.Logging;
using pdfforge.PDFCreator.Threading;
using pdfforge.PDFCreator.Utilities.Communication;

namespace pdfforge.PDFCreator.COM
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("3803F46C-F5AA-4B86-8B9C-6EFFAC9CDCFA")]
    public interface IQueue
    {
        void Initialize();
        bool WaitForJob(int timeOut);
        bool WaitForJobs(int jobCount, int timeOut);
        int Count { get; }
        PrintJob NextJob { get; }
        PrintJob GetJobByIndex(int jobIndex);
        void MergeJobs(PrintJob job1, PrintJob job2);
        void MergeAllJobs();
        void Clear();
        void DeleteJob(int index);
        void ReleaseCom();
    }


    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("66A9CAB1-404A-4918-8DE2-29C26B9B271E")]
    [ProgId("PDFCreator.JobQueue")]
    public class Queue : IQueue
    {
        private static readonly Logger COMLogger = LogManager.GetCurrentClassLogger();
        private readonly AutoResetEvent _newJobResetEvent = new AutoResetEvent(false);
        private JobInfoQueue _comJobInfoQueue;
        private readonly ThreadPool _threadPool = ThreadPool.Instance;
        private bool _isComActive;

        private bool IsServerInstanceRunning
        {
            get
            {
                return PipeServer.SessionServerInstanceRunning(ThreadManager.PipeName);
            }
        }

        /// <summary>
        ///     Initializes the essential components like JobInfoQueue for the COM object
        /// </summary>
        public void Initialize()
        {
            COMLogger.Trace("COM: Starting initialization process");
            _comJobInfoQueue = JobInfoQueue.Instance;
            _comJobInfoQueue.OnNewJobInfo += (sender, eventArgs) => OnNewJob();
            _isComActive = true;

            if (IsServerInstanceRunning)
                throw new InvalidOperationException("Access forbidden. An instance of PDFCreator is currently running.");

            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);
            SettingsHelper.Init();

            COMLogger.Trace("COM: Starting pipe server thread");
            ThreadManager.Instance.StartPipeServerThread();
        }

        /// <summary>
        ///     Waits for exactly one job to enter the queue
        /// </summary>
        /// <param name="timeOut">Duration which the queue should wait for a job</param>
        /// <returns>False, if the duration was exceeded. Otherwise it returns true</returns>
        public bool WaitForJob(int timeOut)
        {
            return WaitForJobs(1, TimeSpan.FromSeconds(timeOut));
        }

        /// <summary>
        ///     Waits for n jobs to enter the queue
        /// </summary>
        /// <param name="jobCount">Number of jobs to wait for</param>
        /// <param name="timeOut">Duration which the queue should wait for the n jobs</param>
        /// <returns>False, if the duration was exceeded. Otherwise it returns true</returns>
        public bool WaitForJobs(int jobCount, int timeOut)
        {
            TimeSpan ts = TimeSpan.FromSeconds(timeOut);
            return WaitForJobs(jobCount, ts);
        }

        /// <summary>
        ///     Waits for n jobs to enter the queue
        /// </summary>
        /// <param name="jobCount">Number of jobs to wait for</param>
        /// <param name="timeOut">Duration which the queue should wait for a job</param>
        /// <returns>
        ///     False, if the duration was exceeded or the queue contains more or as many jobs as specified by jobCount.
        ///     Otherwise it returns true
        /// </returns>
        private bool WaitForJobs(int jobCount, TimeSpan timeOut)
        {
            if (!_isComActive)
                throw new COMException("No COM instance was found. Initialize the object first.");

            COMLogger.Trace("Waiting for {0} jobs for {1} seconds", jobCount, timeOut);

            DateTime maxTime = DateTime.Now + timeOut;

            while (_comJobInfoQueue.Count < jobCount)
            {
                if (DateTime.Now > maxTime)
                    return false;

                _newJobResetEvent.WaitOne(timeOut);
            }
            return true;
        }  

        /// <summary>
        ///     Returns the number of jobs in the queue
        /// </summary>
        public int Count
        {
            get
            {
                return _comJobInfoQueue.Count;
            }
        }

        /// <summary>
        ///     Calls the Set-Method of the AutoResetEvent when a new job enters the queue
        /// </summary>
        private void OnNewJob()
        {
            _newJobResetEvent.Set();
        }

        /// <summary>
        /// Returns the next job in the queue as a ComJob
        /// </summary>
        public PrintJob NextJob
        {
            get
            {
                return JobById(0);
            }
        }

        /// <summary>
        /// Creates the job from the queue by index
        /// </summary>
        /// <param name="jobIndex">Index of the jobinfo in the queue</param>
        /// <returns>The corresponding ComJob</returns>
        public PrintJob GetJobByIndex(int jobIndex)
        {
            return JobById(jobIndex);
        }

        /// <summary>
        /// Creates the job from the queue by index
        /// </summary>
        /// <param name="id">Index of the jobinfo in the queue</param>
        /// <returns>The corresponding ComJob</returns>
        private PrintJob JobById(int id)
        {
            try
            {
                IJobInfo currentJobInfo = _comJobInfoQueue.JobInfos[id];

                var jobTranslations = new JobTranslations();
                jobTranslations.EmailSignature = MailSignatureHelper.ComposeMailSignature();

                IJob currentJob = new GhostscriptJob(currentJobInfo, SettingsHelper.GetDefaultProfile(), JobInfoQueue.Instance, jobTranslations);

                COMLogger.Trace("COM: Creating the ComJob from the queue determined by the index.");
                var indexedJob = new PrintJob(currentJob, _comJobInfoQueue);
                return indexedJob;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new COMException("Invalid index. Please check the index parameter.");
            }
        }

        /// <summary>
        /// Merges two ComJobs
        /// </summary>
        /// <param name="job1">The first job to merge</param>
        /// <param name="job2">The second job to merge</param>
        public void MergeJobs(PrintJob job1, PrintJob job2)
        {
            COMLogger.Trace("COM: Merging two ComJobs.");
            job1.JobInfo.Merge(job2.JobInfo);
            _comJobInfoQueue.Remove(job2.JobInfo);
        }

        /// <summary>
        /// Merges all jobs in the queue
        /// </summary>
        public void MergeAllJobs()
        {
            if(_comJobInfoQueue.Count == 0)
                throw new COMException("The queue must not be empty.");

            COMLogger.Trace("COM: Merging all ComJobs.");
            while (_comJobInfoQueue.Count > 1)
            {
                var firstJob = _comJobInfoQueue.JobInfos[0];
                var nextJob = _comJobInfoQueue.JobInfos[1];

                firstJob.Merge(nextJob);
                _comJobInfoQueue.Remove(nextJob);
            }
        }

        /// <summary>
        /// Remove all elements from the Queue
        /// </summary>
        public void Clear()
        {
            while (_comJobInfoQueue.JobInfos.Count > 0)
            {
                _comJobInfoQueue.Remove(_comJobInfoQueue.JobInfos[0], true);
            }
        }

        /// <summary>
        /// Deletes a chosen print job.
        /// </summary>
        /// <param name="index">Determines the print job to be removed by its position in the queue.</param>
        public void DeleteJob(int index)
        {
            if (index < 0 || index >= Count)
                throw new COMException("The given index was out of range.");

            _comJobInfoQueue.Remove(_comJobInfoQueue.JobInfos[index], true);
        }

        /// <summary>
        /// In case something went wrong, i.e. an exception was thrown, empties the whole queue.
        /// </summary>
        private void EmptyQueue()
        {
            while (_comJobInfoQueue.JobInfos.Count > 0)
            {
                _comJobInfoQueue.Remove(_comJobInfoQueue.JobInfos[0], true);
            }
            _comJobInfoQueue = null;
        }

        /// <summary>
        ///  Shuts down the used instance
        /// </summary>
        public void ReleaseCom()
        {
            if (_isComActive)
            {
                if(!_threadPool.Join(4000))
                    throw new COMException("One of the thread jobs didn't finish within the time out.");

                COMLogger.Trace("COM: Cleaning up COM resources.");
                if(IsServerInstanceRunning)
                    ThreadManager.Instance.PipeServer.Stop();

                _isComActive = false;

                COMLogger.Trace("COM: Emptying queue.");
                EmptyQueue();
            }
            else
            {
                throw new COMException("No COM Instance was found.");
            }
        }
    }
}