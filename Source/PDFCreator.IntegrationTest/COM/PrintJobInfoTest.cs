using NUnit.Framework;
using pdfforge.PDFCreator;
using pdfforge.PDFCreator.COM;

namespace PDFCreator.IntegrationTest.COM
{
    [TestFixture]
    class PrintJobInfoTest
    {
        private Queue _queue;
        private PrintJob _printJob;
        private PrintJobInfo _printJobInfo;

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

            CreateTestPages(1);

            _printJob = _queue.NextJob;
            _printJobInfo = _printJob.PrintJobInfo;
        }

        [TearDown]
        public void TearDown()
        {
            _queue.ReleaseCom();
        }

        [Test]
        public void PrintJobInfo_IfAccessed_NotNull()
        {
            Assert.NotNull(_printJobInfo);
        }

        [Test]
        public void PrintJobInfo_SubjectInitiallyNullOrEmpty()
        {
            var subject = _printJobInfo.Subject;

            Assert.IsTrue(string.IsNullOrEmpty(subject));
        }

        [Test]
        public void PrintJobInfo_KeywordsInitiallyNullOrEmpty()
        {
            var keywords = _printJobInfo.Keywords;

            Assert.IsTrue(string.IsNullOrEmpty(keywords));
        }

        [Test]
        public void PrintJobInfo_PrintJobNameNotNullOrEmpty()
        {
            var jobName = _printJobInfo.PrintJobName;

            Assert.IsFalse(string.IsNullOrEmpty(jobName));
        }

        [Test]
        public void PrintJobInfo_IfTestPageIsPrinted_PrintJobNameContainsTestpage()
        {
            var jobName = _printJobInfo.PrintJobName;

            StringAssert.Contains("testpage", jobName.ToLower());
        }

        [Test]
        public void PrintJobInfo_IfTestPageIsPrinted_PrintJobAuthorNotNullOrEmpty()
        {
            var jobAuthor = _printJobInfo.PrintJobAuthor;

            Assert.IsFalse(string.IsNullOrEmpty(jobAuthor));
        }


        [Test]
        public void PrintJobInfo_IfProducerIsAsked_PropertyContainsPDFCreator()
        {
            var producer = _printJobInfo.Producer;

            StringAssert.Contains("PDFCreator", producer);
        }

        [Test]
        public void PrintJobInfo_IfAuthorIsSet_PropertyContentEqualsAuthor()
        {
            var author = "SomeAuthor";
            _printJobInfo.PrintJobAuthor = author;

            Assert.AreSame(author, _printJobInfo.PrintJobAuthor);
        }

        [Test]
        public void PrintJobInfo_IfSubjectIsSet_PropertyContentEqualsSubject()
        {
            var subject = "SomeSubject";
            _printJobInfo.Subject = subject;

            Assert.AreSame(subject, _printJobInfo.Subject);
        }

        [Test]
        public void PrintJobInfo_IfKeywordsAreSet_PropertyContentEqualsKeywords()
        {
            var keywords = "SomeKeywords";
            _printJobInfo.Keywords = keywords;

            Assert.AreSame(keywords, _printJobInfo.Keywords);
        }

        [Test]
        public void PrintJobInfo_IfNameIsSet_PropertyContentEqualsName()
        {
            var name = "SomeName";
            _printJobInfo.PrintJobName = name;

            Assert.AreSame(name, _printJobInfo.PrintJobName);
        }
    }
}
