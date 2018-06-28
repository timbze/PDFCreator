using System;
using System.Threading;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.ViewModels;

namespace pdfforge.PDFCreator.UI.Presentation.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ISettingsLoader _loader;
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly SettingsProvider _settingsProvider;

        public SettingsManager(SettingsProvider settingsProvider, ISettingsLoader loader, IInstallationPathProvider installationPathProvider)
        {
            _settingsProvider = settingsProvider;
            _loader = loader;
            _installationPathProvider = installationPathProvider;
        }

        public void LoadPdfCreatorSettings()
        {
            // Synchronize across processes with a Mutex to avaoid partial reads/writes
            RunSynchronized(DoLoadPdfCreatorSettings);
        }

        private void DoLoadPdfCreatorSettings()
        {
            var settings = _loader.LoadPdfCreatorSettings();
            _settingsProvider.UpdateSettings(settings);

            LoggingHelper.ChangeLogLevel(settings.ApplicationSettings.LoggingLevel);
        }

        public ISettingsProvider GetSettingsProvider()
        {
            return _settingsProvider;
        }

        public void SaveCurrentSettings()
        {
            // Synchronize across processes with a Mutex to avoid partial reads/writes
            RunSynchronized(SaveSettings);
        }

        private void SaveSettings()
        {
            var settings = _settingsProvider.Settings;
            _loader.SaveSettingsInRegistry(settings);
        }

        public void ApplyAndSaveSettings(PdfCreatorSettings settings)
        {
            _settingsProvider.UpdateSettings(settings);
            SaveCurrentSettings();
        }

        public void LoadAllSettings()
        {
            LoadPdfCreatorSettings();
        }

        private void RunSynchronized(Action action)
        {
            var mutexName = "PDFCreator-Settings" + _installationPathProvider.ApplicationGuid;
            var mutex = new Mutex(false, mutexName);

            // we have two try blocks, because we can ignore the AbandonedMutexException
            // The action must be called and in case of an exception, ReleaseMutex must be called
            try
            {
                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException)
                {
                }

                action();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
