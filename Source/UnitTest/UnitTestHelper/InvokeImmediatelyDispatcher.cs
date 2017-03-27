using System;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class InvokeImmediatelyDispatcher : IDispatcher
    {
        public void BeginInvoke(Action action)
        {
            action();
        }

        public void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo)
        {
            addMethod(jobInfo);
        }
    }
}
