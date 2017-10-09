using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellLauncher : IMainWindowThreadLauncher
    {
        private readonly IShellManager _shellManager;
        private readonly IThreadManager _threadManager;
        private StartupRoutine _routine;
        private ISynchronizedThread _mainWindowThread;

        public MainShellLauncher(IThreadManager threadManager, IShellManager shellManager)
        {
            _threadManager = threadManager;
            _shellManager = shellManager;
        }

        public void LaunchMainWindow(StartupRoutine startup)
        {
            if (_mainWindowThread != null)
            {
                _shellManager.MainShellToFront();
                return;
            }
            _routine = startup;
            _mainWindowThread = _threadManager.StartSynchronizedUiThread(MainWindowLaunchThreadMethod, "MainWindowThread");
        }

        private void MainWindowLaunchThreadMethod()
        {
            try
            {
                if (_routine == null)
                {
                    _routine = new StartupRoutine();
                    _routine.AddAction(new StartupNavigationAction(RegionNames.MainRegion, MainRegionViewNames.HomeView));
                }

                _shellManager.ShowMainShell(_routine);
            }
            finally
            {
                _mainWindowThread = null;
            }
        }
    }
}
