using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public abstract class SettingsProvider : ISettingsProvider
    {
        public PdfCreatorSettings Settings { get; private set; }

        public abstract IGpoSettings GpoSettings { get; }

        public event EventHandler LanguageChanged;

        public abstract string GetApplicationLanguage();

        public bool CheckValidSettings(PdfCreatorSettings settings)
        {
            return settings.ConversionProfiles.Count > 0;
        }

        public ConversionProfile GetDefaultProfile()
        {
            return Settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);
        }

        private void RaiseLanguageChanged()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateSettings(PdfCreatorSettings settings)
        {
            var oldLanguage = Settings?.ApplicationSettings.Language;

            Settings = settings;

            if (!settings.ApplicationSettings.Language.Equals(oldLanguage))
                RaiseLanguageChanged();
        }
    }

    public class DefaultSettingsProvider : SettingsProvider
    {
        public override IGpoSettings GpoSettings => new GpoSettingsDefaults();

        public override string GetApplicationLanguage()
        {
            return Settings == null ? "en" : Settings.ApplicationSettings.Language;
        }
    }
}
