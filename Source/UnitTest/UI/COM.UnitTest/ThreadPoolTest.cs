using System.Threading;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Threading;
using ThreadPool = pdfforge.PDFCreator.Core.ComImplementation.ThreadPool;

namespace pdfforge.PDFCreator.UnitTest.COM
{
    [TestFixture]
    internal class ThreadPoolTest
    {
        [Test]
        public void AddThread_IfNoThreadRunning_ThreadIsStarted()
        {
            var testThread = Substitute.For<ISynchronizedThread>();
            testThread.ThreadState.Returns(ThreadState.Unstarted);

            var threadPool = new ThreadPool();

            threadPool.AddThread(testThread);

            testThread.Received().Start();
        }

        [Test]
        public void AddThread_IfThreadIsAlreadyRunning_EnqueueIncomingThreads()
        {
            var testThread1 = Substitute.For<ISynchronizedThread>();
            testThread1.ThreadState.Returns(ThreadState.Unstarted);
            var testThread2 = Substitute.For<ISynchronizedThread>();
            testThread2.ThreadState.Returns(ThreadState.Unstarted);
            var threadPool = new ThreadPool();

            threadPool.AddThread(testThread1);
            threadPool.AddThread(testThread2);

            testThread2.DidNotReceive().Start();
        }
    }
}