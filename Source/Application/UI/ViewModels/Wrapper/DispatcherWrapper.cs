using System;
using System.Windows.Threading;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;

namespace pdfforge.PDFCreator.UI.ViewModels.Wrapper
{
    public interface IDispatcher
    {
        void BeginInvoke(Action action);
        void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo);
    }

    public class DispatcherWrapper : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        public void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo)
        {
            _dispatcher.BeginInvoke(addMethod, jobInfo);
        }
    }
}