using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsProvider
    {
        PdfCreatorSettings Settings { get; }
        IGpoSettings GpoSettings { get; }

        event EventHandler LanguageChanged;

        ConversionProfile GetDefaultProfile();

        string GetApplicationLanguage();

        bool CheckValidSettings(PdfCreatorSettings settings);
    }
}