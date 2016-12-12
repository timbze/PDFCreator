using System.Collections.Generic;
using System.Globalization;
using pdfforge.DynamicTranslator;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public interface ILanguageProvider
    {
        IEnumerable<Language> GetAvailableLanguages();
        bool HasTranslation(string language);
        Language FindBestLanguage(CultureInfo culture);
    }
}