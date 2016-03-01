using System;
using pdfforge.PDFCreator.Core.Jobs;

namespace pdfforge.PDFCreator
{
    /// <summary>
    /// EventArgs class that contains the new JobInfo
    /// </summary>
    public class NewJobInfoEventArgs : EventArgs
    {
        public IJobInfo JobInfo { get; private set; }

        public NewJobInfoEventArgs(IJobInfo jobInfo)
        {
            JobInfo = jobInfo;
        }
    }
}