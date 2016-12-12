using System;
using System.Windows.Threading;

namespace pdfforge.PDFCreator.UI.ViewModels.Wrapper
{
    public interface IDispatcher
    {
        void Invoke(Action action);

        DispatcherOperation BeginInvoke(Action action);
    }

    public class DispatcherWrapper : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }

        public DispatcherOperation BeginInvoke(Action action)
        {
            return _dispatcher.BeginInvoke(action);
        }
    }
}