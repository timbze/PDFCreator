using NLog;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Help;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public partial class App : Application
    {
        private readonly IAppStart _appStart;
        private readonly ISettingsManager _settingsManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public App(IAppStart appStart, HelpCommandHandler helpCommandHandler, ISettingsManager settingsManager)
        {
            _appStart = appStart;
            _settingsManager = settingsManager;
            InitializeComponent();
            helpCommandHandler.RegisterHelpCommandBinding();
            RegisterXamlCulture();
        }

        private void Shutdown(ExitCode exitCode)
        {
            Shutdown((int)exitCode);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                if (!_appStart.SkipStartupConditionCheck)
                {
                    // Load settings to have the proper translation available
                    _settingsManager.LoadAllSettings();

                    _appStart.CheckApplicationConditions();
                }
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

        private void RegisterXamlCulture()
        {
            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}
