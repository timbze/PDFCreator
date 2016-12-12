using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class SpooledJobFinderTest
    {
        [SetUp]
        public void Setup()
        {
            _spoolFolder = "SomeFolder";
            var spoolerProvider = Substitute.For<ISpoolerProvider>();
            spoolerProvider.SpoolFolder.Returns(_spoolFolder);
            _dir = Substitute.For<IDirectory>();
            _jobManager = Substitute.For<IJobInfoManager>();

            _spooledJobFinder = new SpooledJobFinder(spoolerProvider, _dir, _jobManager);
        }

        private IDirectory _dir;
        private string _spoolFolder;
        private IJobInfoManager _jobManager;
        private SpooledJobFinder _spooledJobFinder;

        [Test]
        public void ForEachInfFile_AddsJobInfo()
        {
            var files = new[] {"one.inf", "two.inf", "three.inf", "other.inf"};
            _dir.Exists(_spoolFolder).Returns(true);

            _dir.GetFiles(_spoolFolder, "*.inf", SearchOption.AllDirectories).Returns(files);

            _jobManager.ReadFromInfFile(Arg.Any<string>()).Returns(x => new JobInfo {InfFile = x.Arg<string>()});

            var jobs = _spooledJobFinder.GetJobs();
            Assert.AreEqual(files.Length, jobs.Count());

            foreach (var file in files)
            {
                Assert.IsTrue(jobs.Any(x => x.InfFile.Equals(file)));
            }
        }

        [Test]
        public void IfDirDoesNotExist_ReturnsEmtpyList()
        {
            _dir.Exists(_spoolFolder).Returns(false);
            Assert.IsEmpty(_spooledJobFinder.GetJobs());
        }

        [Test]
        public void IfReadingInfThrowsException_DoesNotThrow()
        {
            var files = new[] {"one.inf", "two.inf", "three.inf", "other.png"};
            _dir.Exists(_spoolFolder).Returns(true);

            _dir.GetFiles(_spoolFolder, "*.inf", SearchOption.AllDirectories).Returns(files);

            _jobManager.ReadFromInfFile(Arg.Any<string>()).Throws(new Exception("something went wrong"));

            var jobs = _spooledJobFinder.GetJobs();
            Assert.AreEqual(0, jobs.Count());
        }
    }
}