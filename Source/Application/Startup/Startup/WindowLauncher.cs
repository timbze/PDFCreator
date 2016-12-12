using pdfforge.Obsidian;
using pdfforge.PDFCreator.Interactions;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.Startup
{
    public class WindowLauncher : IWindowLauncher
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IThreadManager _threadManager;

        private ISynchronizedThread _mainWindowThread;

        public WindowLauncher(IThreadManager threadManager, IInteractionInvoker interactionInvoker)
        {
            _threadManager = threadManager;
            _interactionInvoker = interactionInvoker;
        }

        public void LaunchMainWindow()
        {
            if (_mainWindowThread != null)
                return;

            _mainWindowThread = _threadManager.StartSynchronizedUiThread(MainWindowLaunchThreadMethod, "MainWindowThread");
        }

        private void MainWindowLaunchThreadMethod()
        {
            try
            {
                _interactionInvoker.Invoke(new MainWindowInteraction());
            }
            finally
            {
                _mainWindowThread = null;
            }
        }
    }
}