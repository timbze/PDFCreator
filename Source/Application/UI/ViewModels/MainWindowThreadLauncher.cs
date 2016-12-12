using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.ViewModels
{
    public class MainWindowThreadLauncher : IMainWindowThreadLauncher
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IThreadManager _threadManager;

        private ISynchronizedThread _mainWindowThread;

        public MainWindowThreadLauncher(IThreadManager threadManager, IInteractionInvoker interactionInvoker)
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