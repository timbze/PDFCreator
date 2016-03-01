using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Jobs;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Jobs
{
    [TestFixture]
    public class JobInfoTest
    {
        [Test]
        public void JobInfo_WhenMergedWithOtherJobInfo_DeleteIsCalledOnSecondJob()
        {
            IJobInfo jobInfo = new JobInfo();
            IJobInfo jobInfoStub = MockRepository.GenerateStub<IJobInfo>();

            jobInfoStub.Stub(x => x.SourceFiles).Return(new List<SourceFileInfo>());
            
            jobInfo.Merge(jobInfoStub);

            jobInfoStub.AssertWasCalled(x => x.Delete(false));
        }

        [Test]
        public void JobInfo_WhenMergedWithOtherJobInfo_MergedJobContainsBothSourceFiles()
        {
            IJobInfo jobInfo = new JobInfo();
            jobInfo.SourceFiles.Add(FakeSourceFileInfo());

            IJobInfo jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            jobInfoStub.Stub(x => x.SourceFiles).Return(new List<SourceFileInfo>(new[] { FakeSourceFileInfo()}));

            jobInfo.Merge(jobInfoStub);

            Assert.AreEqual(2, jobInfo.SourceFiles.Count);
        }

        [Test]
        public void Merge_IfJobTypesAreDifferent_ReturnsWithoutMerging()
        {
            IJobInfo jobInfo = new JobInfo();
            jobInfo.JobType = JobType.XpsJob;

            IJobInfo jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            jobInfoStub.JobType = JobType.PsJob;

            jobInfoStub.Stub(x => x.SourceFiles).Return(new List<SourceFileInfo>());

            jobInfo.Merge(jobInfoStub);

            jobInfoStub.AssertWasNotCalled(x => x.Delete(false));
        }

        [Test]
        public void JobInfo_AfterMerge_SourceFilesHaveCorrectOrder()
        {
            var sf1 = FakeSourceFileInfo();
            var sf2 = FakeSourceFileInfo();

            IJobInfo jobInfo = new JobInfo();
            jobInfo.SourceFiles.Add(sf1);

            IJobInfo jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            jobInfoStub.Stub(x => x.SourceFiles).Return(new List<SourceFileInfo>(new[] { sf2 }));

            jobInfo.Merge(jobInfoStub);

            Assert.AreSame(sf1, jobInfo.SourceFiles[0]);
            Assert.AreSame(sf2, jobInfo.SourceFiles[1]);
        }

        [Test]
        public void JobInfoWithMultipleSourceFiles_AfterMerge_SourceFilesHaveCorrectOrder()
        {
            var sourceFiles = new List<SourceFileInfo>();
            for (int i = 0; i < 8; i++)
            {
                sourceFiles.Add(FakeSourceFileInfo());
            }

            IJobInfo jobInfo = new JobInfo();
            for (int i = 0; i < 4; i++)
            {
                jobInfo.SourceFiles.Add(sourceFiles[i]);
            }

            IJobInfo jobInfoStub = MockRepository.GenerateStub<IJobInfo>();
            jobInfoStub.Stub(x => x.SourceFiles).Return(sourceFiles.GetRange(4, 4));

            jobInfo.Merge(jobInfoStub);

            for (int i = 0; i < 8; i++)
            {

                Assert.AreSame(sourceFiles[i], jobInfo.SourceFiles[i]);
            }
        }

        private SourceFileInfo FakeSourceFileInfo()
        {
            var source = new SourceFileInfo();

            return source;
        }
    }
}
