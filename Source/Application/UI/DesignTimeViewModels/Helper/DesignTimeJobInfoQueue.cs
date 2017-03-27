using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeJobInfoQueue : IJobInfoQueue
    {
        public IList<JobInfo> JobInfos { get; } = new List<JobInfo>();
        public int Count { get; } = 0;
        public JobInfo NextJob { get; }
        public bool IsEmpty { get; } = true;
        public event EventHandler<NewJobInfoEventArgs> OnNewJobInfo;

        public bool Remove(JobInfo jobInfo, bool deleteFiles)
        {
            return true;
        }

        public void Add(IEnumerable<JobInfo> jobInfos)
        {
            
        }

        public bool Remove(JobInfo jobInfo)
        {
            return true;
        }

        public void Add(JobInfo jobInfo)
        {
        }
    }
}