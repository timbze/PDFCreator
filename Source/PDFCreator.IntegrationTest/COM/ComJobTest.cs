using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using pdfforge.PDFCreator;
using pdfforge.PDFCreator.COM;

namespace PDFCreator.IntegrationTest.COM
{
    [TestFixture]
    class ComJobTest
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
        [ExpectedException(typeof(ArgumentException))]                  //Tests for exceptions thrown by the framework can stay this way
        public void ConvertTo_IfFilenameEmpty_ThrowsArgumentException()
        {
            CreateTestPages(1);
            
            const string filename = "";
            PrintJob comJob = _queue.NextJob;

            comJob.ConvertTo(filename);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTo_IfFilenameHasIllegalChars_ThrowsArgumentException()
        {
            CreateTestPages(1);

            const string filename = "testpage>testpage.pdf";
            PrintJob comJob = _queue.NextJob;

            comJob.ConvertTo(filename);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTo_IfFilenameHasIllegalExtension_ThrowsArgumentException()
        {
            CreateTestPages(1);

            const string filename = "testpage\testpage.adfsd";
            var comJob = _queue.NextJob;

            comJob.ConvertTo(filename);
        }

        [Test]
        public void ConvertTo_IfFilenameDirectoryNotExisting_ThrowsCOMException()
        {
            CreateTestPages(1);

            const string filename = "basdeead\\aokdeaad.pdf";
            var comJob = _queue.NextJob;

            var ex = Assert.Throws<COMException>(() => comJob.ConvertTo(filename));
            StringAssert.Contains("Invalid path. Please check if the directory exists.", ex.Message);
        }

        [Test]
        public void ProfileSettings_IfEmptyPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var ex = Assert.Throws<COMException>(() => comJob.SetProfileSetting("","True"));
            StringAssert.Contains("Invalid property name.", ex.Message);
        }

        [Test]
        public void ProfileSettings_IfNotExistingPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var ex = Assert.Throws<COMException>(() => comJob.SetProfileSetting("NotExisting", "True"));
            StringAssert.Contains("Invalid property name.", ex.Message);
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void SetProfileSettings_IfEmptyValue_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            comJob.SetProfileSetting("PdfSettings.Security.Enabled", "");
        }

        [Test]
        [ExpectedException(typeof (FormatException))]
        public void SetProfileSettings_IfInappropriateValue_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            comJob.SetProfileSetting("PdfSettings.Security.Enabled","3");
        }

        [Test]
        public void GetProfileSettings_IfEmptyPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var ex = Assert.Throws<COMException>(() => comJob.GetProfileSetting(""));
            StringAssert.Contains("Invalid property name.", ex.Message);
        }

        [Test]
        public void GetProfileSettings_IfInvalidPropertyname_ThrowsCOMException()
        {
            CreateTestPages(1);

            var comJob = _queue.NextJob;
            var ex = Assert.Throws<COMException>(() => comJob.GetProfileSetting("asdioajsd"));
            StringAssert.Contains("Invalid property name.", ex.Message);
        }
    }
}
