using System;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;

namespace pdfforge.PDFCreator.Core.Workflow
{
    /// <summary>
    ///     EventArgs class that contains the new JobInfo
    /// </summary>
    public class NewJobInfoEventArgs : EventArgs
    {
        public NewJobInfoEventArgs(JobInfo jobInfo)
        {
            JobInfo = jobInfo;
        }

        public JobInfo JobInfo { get; private set; }
    }
}