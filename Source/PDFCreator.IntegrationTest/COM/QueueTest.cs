using System;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;
using pdfforge.PDFCreator;
using pdfforge.PDFCreator.COM;

namespace PDFCreator.IntegrationTest.COM
{
    [TestFixture]
    class QueueTest
    {
        private Queue _queue;
  
        private void CreateTestPages(int n)
        {
            for (int i = 0; i < n; i++)
            {
                JobInfoQueue.Instance.AddTestPage();
            }
        }

        [SetUp]
        public void SetUp()
        {
            _queue = new Queue();
            _queue.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _queue.ReleaseCom();
        }

        [Test]
        public void GetJobByIndex_IfIndexOutOfRange_ThrowsCOMException()
        {
            CreateTestPages(3);

            var ex = Assert.Throws<COMException>(() => _queue.GetJobByIndex(3));
            StringAssert.Contains("Invalid index. Please check the index parameter.", ex.Message);
        }

        [Test]
        public void MergeAllJobs_IfQueueEmpty_ThrowsCOMException()
        {
            var ex = Assert.Throws<COMException>(() => _queue.MergeAllJobs());
            StringAssert.Contains("The queue must not be empty.", ex.Message);
        }


        [Test]
        public void MergeAllJobs_IfQueueCountGreater1_QueueCountEquals1()
        {
            CreateTestPages(4);
            _queue.MergeAllJobs();

            int jobCount = _queue.Count;
            
            Assert.AreEqual(1, jobCount);
        }

        [Test]
        public void MergeAllJobs_IfQueueCountExactly1_QueueCountEquals1()
        {
            CreateTestPages(1);
            _queue.MergeAllJobs();

            int jobCount = _queue.Count;

            Assert.AreEqual(1,jobCount);
        }

        [Test]
        public void WaitForJobs_IfLessJobsEnteredThanExpected_ReturnsFalse()
        {
            var aThread = new Thread(() => CreateTestPages(5));
            aThread.Start();
            var hasTooFewEntered = !_queue.WaitForJobs(6, 1);

            Assert.IsTrue(hasTooFewEntered);
        }

        [Test]
        public void WaitForJobs_IfTimeoutOver_ReturnFalse()
        {
            var isNotTimedOut = _queue.WaitForJobs(2, 1);

            Assert.IsFalse(isNotTimedOut);
        }

        //This test is trivial since without any implementation of _queue.Clear(), it works.
        //But it shows, if the implementation fullfills trivial conditions
        [Test]
        public void Clear_IfQueueEmpty_DoNothing()
        {
            _queue.Clear();
            var isQueueZero = _queue.Count == 0;

            Assert.IsTrue(isQueueZero);
        }

        [Test]
        public void Clear_IfQueueHasElements_CountEquals0()
        {
            //Create meanless print jobs 
            CreateTestPages(5);
            _queue.Clear();

            var isQueueZero = _queue.Count == 0;
            
            Assert.IsTrue(isQueueZero);
        }

        [Test]
        public void DeleteJob_IfIndexIsNegativ_ThrowCOMExeption()
        {
            CreateTestPages(2);

            var ex = Assert.Throws<COMException>(() => _queue.DeleteJob(-1));
            StringAssert.Contains("The given index was out of range.", ex.Message);
        }

        [Test]
        public void DeleteJob_IfIndexTooBig_ThrowCOMExeption()
        {
            CreateTestPages(2);

            var ex = Assert.Throws<COMException>(() => _queue.DeleteJob(3));
            StringAssert.Contains("The given index was out of range.", ex.Message);
        }

        [Test]
        public void DeleteJob_ReducedCountAfterDeletion()
        {
            CreateTestPages(4);

            _queue.DeleteJob(3);

            var count = _queue.Count;

            Assert.AreEqual(3, count);
        }
    }
}
