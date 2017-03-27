using System.Windows;
using NLog;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UI.Views
{
    public partial class App : Application
    {
        private readonly IAppStart _appStart;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App(IAppStart appStart)
        {
            _appStart = appStart;
            InitializeComponent();
        }

        private void Shutdown(ExitCode exitCode)
        {
            Shutdown((int) exitCode);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                if (!_appStart.SkipStartupConditionCheck)
                    _appStart.CheckApplicationConditions();
            }
            catch (StartupConditionFailedException ex)
            {
                _logger.Error($"Error while starting the application: {ex.Message} ({ex.ExitCode})");
                Shutdown(ex.ExitCode);
                return;
            }

            var exitCode = _appStart.Run();
            Shutdown(exitCode);
        }
    }
}
