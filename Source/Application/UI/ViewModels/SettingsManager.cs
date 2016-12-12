using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.ViewModels
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ISettingsLoader _loader;
        private readonly SettingsProvider _settingsProvider;

        public SettingsManager(SettingsProvider settingsProvider, ISettingsLoader loader)
        {
            _settingsProvider = settingsProvider;
            _loader = loader;
        }

        public void LoadPdfCreatorSettings()
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
    }
}