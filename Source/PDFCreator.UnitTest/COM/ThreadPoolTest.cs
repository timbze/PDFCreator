using NUnit.Framework;
using pdfforge.PDFCreator.COM;
using pdfforge.PDFCreator.Utilities.Threading;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.COM
{
    [TestFixture]
    class ThreadPoolTest
    {
        private readonly ThreadPool _testThreadPool = ThreadPool.Instance;

        private ISynchronizedThread CreateSyncThreadMock(bool endsImmediately)
        {
            var threadMock = MockRepository.GenerateStub<ISynchronizedThread>();

            if (endsImmediately)
            {
                threadMock.Stub(x => x.Start())
                    .WhenCalled(
                        delegate
                        {
                            threadMock.Raise(x => x.OnThreadFinished += null, threadMock,
                                new ThreadFinishedEventArgs(threadMock));
                        });
            }

            return threadMock;
        }

        [Test]
        public void AddThread_IfNoThreadRunning_ThreadIsStarted()
        {
            var testThread = CreateSyncThreadMock(false);

            _testThreadPool.AddThread(testThread);

            testThread.AssertWasCalled(x => x.Start());
        }

        [Test]
        public void AddThread_IfThreadIsAlreadyRunning_EnqueueIncomingThreads()
        {
            var testThread1 = CreateSyncThreadMock(false);
            var testThread2 = CreateSyncThreadMock(true);

            _testThreadPool.AddThread(testThread1);
            _testThreadPool.AddThread(testThread2);
            testThread2.AssertWasNotCalled(x => x.Start());
        }
    }
}
