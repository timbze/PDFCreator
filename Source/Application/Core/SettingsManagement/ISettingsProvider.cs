using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsProvider : IApplicationLanguageProvider
    {
        PdfCreatorSettings Settings { get; }
        IGpoSettings GpoSettings { get; }


        ConversionProfile GetDefaultProfile();


        bool CheckValidSettings(PdfCreatorSettings settings);
    }

    public interface IApplicationLanguageProvider
    {
        event EventHandler LanguageChanged;

        string GetApplicationLanguage();
    }
}