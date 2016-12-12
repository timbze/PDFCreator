using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsManager
    {
        void LoadPdfCreatorSettings();
        ISettingsProvider GetSettingsProvider();
        void SaveCurrentSettings();
        void ApplyAndSaveSettings(PdfCreatorSettings settings);
        void LoadAllSettings();
    }
}