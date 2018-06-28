using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ISettingsChanged
    {
        bool HaveChanged();
    }

    public class SettingsChanged : ISettingsChanged
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ILanguageProvider _languageProvider;
        private readonly IGpoSettings _gpoSettings;

        public SettingsChanged(ICurrentSettingsProvider currentSettingsProvider, ISettingsProvider settingsProvider,
            ILanguageProvider languageProvider, IGpoSettings gpoSettings)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _settingsProvider = settingsProvider;
            _languageProvider = languageProvider;
            _gpoSettings = gpoSettings;
        }

        public bool HaveChanged()
        {
            var currentSettings = _currentSettingsProvider.Settings;
            var storedSettings = _settingsProvider.Settings;

            if (_gpoSettings.Language == null)
                if (currentSettings.ApplicationSettings.Language != _languageProvider.CurrentLanguage.Iso2)
                {
                    return true;
                }

            return !currentSettings.Equals(storedSettings);
        }
    }
}
