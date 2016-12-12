using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.Core.GpoAdapter
{
    public class GpoAwareSettingsProvider : SettingsProvider
    {
        public GpoAwareSettingsProvider()
        {
            var gpoReader = new GpoReader.GpoReader(true);
            GpoSettings = new GpoReaderSettings(gpoReader.ReadGpoSettings());
        }

        public override IGpoSettings GpoSettings { get; }

        public override string GetApplicationLanguage()
        {
            return string.IsNullOrWhiteSpace(GpoSettings.Language)
                ? Settings.ApplicationSettings.Language
                : GpoSettings.Language;
        }
    }
}
