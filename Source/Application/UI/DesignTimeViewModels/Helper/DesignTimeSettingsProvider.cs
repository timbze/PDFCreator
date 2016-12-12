using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    internal class DesignTimeSettingsProvider : ISettingsProvider
    {
        public DesignTimeSettingsProvider()
        {
            Settings = new PdfCreatorSettings(null);
            Settings.ConversionProfiles.Add(new ConversionProfile());
        }

        public PdfCreatorSettings Settings { get; }
        public IGpoSettings GpoSettings { get; }
        public event EventHandler LanguageChanged;

        public ConversionProfile GetDefaultProfile()
        {
            return Settings.ConversionProfiles[0];
        }

        public string GetApplicationLanguage()
        {
            return "English";
        }

        public bool CheckValidSettings(PdfCreatorSettings settings)
        {
            return true;
        }
    }
}