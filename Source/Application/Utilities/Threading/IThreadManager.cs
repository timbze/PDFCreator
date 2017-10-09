using System;
using System.Threading;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    public interface IThreadManager
    {
        Action UpdateAfterShutdownAction { get; set; }

        event EventHandler<ThreadFinishedEventArgs> CleanUpAfterThreadClosed;

        void StartSynchronizedThread(ISynchronizedThread thread);

        ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName);

        ISynchronizedThread StartSynchronizedUiThread(ThreadStart threadMethod, string threadName);

        void Shutdown();

        void WaitForThreads();
    }
}
