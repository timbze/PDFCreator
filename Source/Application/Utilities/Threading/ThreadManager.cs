using System;
using System.Collections.Generic;
using System.Threading;
using SystemInterface;
using NLog;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    /// <summary>
    ///     The ThreadManager class handles and watches all applications threads. If all registered threads are finished, the
    ///     application will exit.
    /// </summary>
    public class ThreadManager : IThreadManager
    {
        private static readonly object LockObject = new object();
        private readonly IEnvironment _environment;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly List<ISynchronizedThread> _threads = new List<ISynchronizedThread>();

        private bool _isShuttingDown;

        public ThreadManager(IEnvironment environment)
        {
            _environment = environment;
        }

        public Action UpdateAfterShutdownAction { get; set; }

        /// <summary>
        ///     Adds and starts a synchronized thread to the thread list. The application will wait for all of these to end before
        ///     it terminates
        /// </summary>
        /// <param name="thread">A thread that needs to be synchronized. This thread will not be automatically started</param>
        public void StartSynchronizedThread(ISynchronizedThread thread)
        {
            lock (LockObject)
            {
                if (_isShuttingDown)
                {
                    _logger.Warn("Tried to start thread while shutdown already started!");
                    return;
                }

                _logger.Debug("Adding thread " + thread.Name);

                _threads.Add(thread);
                thread.OnThreadFinished += thread_OnThreadFinished;

                if (thread.ThreadState == ThreadState.Unstarted)
                    thread.Start();
            }
        }

        public ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName)
        {
            return StartSynchronizedThread(threadMethod, threadName, ApartmentState.MTA);
        }

        public ISynchronizedThread StartSynchronizedUiThread(ThreadStart threadMethod, string threadName)
        {
            return StartSynchronizedThread(threadMethod, threadName, ApartmentState.STA);
        }

        /// <summary>
        ///     Wait for all Threads and exit the application afterwards
        /// </summary>
        public void WaitForThreads()
        {
            CleanUpThreads();

            _logger.Debug("Waiting for all synchronized threads to end");

            while (_threads.Count > 0)
            {
                _logger.Debug(_threads.Count + " Threads remaining");

                try
                {
                    _threads[0].Join();
                }
                catch (ArgumentOutOfRangeException)
                {
                } // thread has been removed just after checking while condition

                CleanUpThreads();
            }

            _logger.Debug("All synchronized threads have ended");
        }

        public void Shutdown()
        {
            lock (LockObject)
            {
                _logger.Debug("Shutting down the application");
                _isShuttingDown = true;

                // convert _threads to array to prevent InvalidOperationException when an item is removed
                foreach (var t in _threads.ToArray())
                {
                    if (string.IsNullOrEmpty(t.Name))
                        _logger.Debug("Aborting thread");
                    else
                        _logger.Debug("Aborting thread " + t.Name);

                    t.Abort();
                }

                _logger.Debug("Exiting...");

                if (UpdateAfterShutdownAction != null)
                {
                    _logger.Debug("Starting application update...");
                    UpdateAfterShutdownAction();
                }
            }
        }

        private ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName, ApartmentState state)
        {
            _logger.Debug($"Starting {threadName} thread");

            var t = new SynchronizedThread(threadMethod);
            t.Name = threadName;
            t.SetApartmentState(state);

            StartSynchronizedThread(t);
            return t;
        }

        /// <summary>
        ///     Remove all finished threads
        /// </summary>
        private void CleanUpThreads()
        {
            lock (LockObject)
            {
                try
                {
                    _threads.RemoveAll(t => t.IsAlive == false);
                }
                catch (NullReferenceException ex)
                {
                    _logger.Warn(ex, "There was an exception while cleaning up threads");
                }
            }
        }

        /// <summary>
        ///     Remove threads when they have finished
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">EventArgs with information about the thread</param>
        private void thread_OnThreadFinished(object sender, ThreadFinishedEventArgs e)
        {
            try
            {
                lock (LockObject)
                {
                    _threads.Remove(e.SynchronizedThread);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}