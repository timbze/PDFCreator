using System.Collections.Generic;
using pdfforge.DynamicTranslator;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class TranslationProxy : TranslatorBase
    {
        public ITranslator Translator { get; set; }

        public override string LanguageName => Translator == null ? "" : Translator.LanguageName;

        protected override string GetRawTranslation(string section, string item)
        {
            if (Translator == null)
                return "";

            return Translator.GetTranslation(section, item);
        }

        public override IList<string> GetKeysForSection(string section)
        {
            if (Translator == null)
                return new List<string>();

            return Translator.GetKeysForSection(section);
        }
    }
}