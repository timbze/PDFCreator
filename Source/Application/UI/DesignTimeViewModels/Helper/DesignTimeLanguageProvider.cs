using System;
using System.Collections.Generic;
using System.Globalization;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    internal class DesignTimeLanguageProvider : ILanguageProvider
    {
        public IEnumerable<Language> GetAvailableLanguages()
        {
            return new List<Language>();
        }

        public bool HasTranslation(string language)
        {
            return true;
        }

        public Language FindBestLanguage(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Language FindBestLanguage(string language)
        {
            throw new NotImplementedException();
        }
    }
}