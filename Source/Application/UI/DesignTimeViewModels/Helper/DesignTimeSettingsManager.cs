using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeSettingsManager : ISettingsManager
    {
        public void LoadPdfCreatorSettings()
        {
        }

        public ISettingsProvider GetSettingsProvider()
        {
            return new DesignTimeSettingsProvider();
        }

        public void SaveCurrentSettings()
        {
        }

        public void ApplyAndSaveSettings(PdfCreatorSettings settings)
        {
            throw new NotImplementedException();
        }

        public void LoadAllSettings()
        {
        }

        public void LoadGpoSettings()
        {
        }
    }
}