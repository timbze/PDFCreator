using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Shared.Helper;

namespace PDFCreator.Shared.Test
{
    [TestFixture]
    public class TranslationUtilTest
    {
        [Test]
        public void FindBestLanguage_WithExistingLcid_ReturnsProperLanguage()
        {
            var translations = new List<Language>();
            translations.Add(new Language { Iso2 = "en", LanguageId = "0x0409" });
            translations.Add(new Language { Iso2 = "de", LanguageId = "0x0007" });
            translations.Add(new Language { Iso2 = "nl", LanguageId = "0x0013" });

            var ci = CultureInfo.GetCultureInfo("de");

            var language = TranslationHelper.Instance.LanguageLoader.FindBestLanguage(ci, translations);

            Assert.AreEqual(translations[1], language);
        }

        [Test]
        public void FindBestLanguage_WithNonexistingLcidButSameIso2_ReturnsProperLanguage()
        {
            var translations = new List<Language>();
            translations.Add(new Language { Iso2 = "en", LanguageId = "0x0409" });
            translations.Add(new Language { Iso2 = "de", LanguageId = "0x0007" });
            translations.Add(new Language { Iso2 = "nl", LanguageId = "0x0013" });

            var ci = CultureInfo.GetCultureInfo("de-DE");

            var language = TranslationHelper.Instance.LanguageLoader.FindBestLanguage(ci, translations);

            Assert.AreEqual(translations[1], language);
        }

        [Test]
        public void FindBestLanguage_WithNonexistingLcid_ReturnsEnglishLanguage()
        {
            var translations = new List<Language>();
            translations.Add(new Language { Iso2 = "en", LanguageId = "0x0409" });
            translations.Add(new Language { Iso2 = "nl", LanguageId = "0x0013" });

            var ci = CultureInfo.GetCultureInfo("de");

            var language = TranslationHelper.Instance.LanguageLoader.FindBestLanguage(ci, translations);

            Assert.AreEqual(translations[0], language);
        }

        [Test]
        public void FindBestLanguage_WithNonexistingLcidAndWithoutEnglish_ReturnsNull()
        {
            var translations = new List<Language>();
            translations.Add(new Language { Iso2 = "nl", LanguageId = "0x0013" });

            var ci = CultureInfo.GetCultureInfo("de");

            var language = TranslationHelper.Instance.LanguageLoader.FindBestLanguage(ci, translations);

            Assert.IsNull(language);
        }
    }
}
