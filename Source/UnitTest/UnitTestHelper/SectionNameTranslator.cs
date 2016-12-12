using System;
using System.Collections.Generic;
using pdfforge.DynamicTranslator;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class SectionNameTranslator : ITranslator
    {
        public IEnumerable<EnumValue<T>> GetEnumTranslation<T>() where T : struct, IConvertible
        {
            throw new NotImplementedException();
        }

        public string GetFormattedTranslation(string section, string item, params object[] args)
        {
            return $"{section}\\{item}";
        }

        public string GetTranslation(string section, string item)
        {
            return $"{section}\\{item}";
        }

        public IList<string> GetKeysForSection(string section)
        {
            throw new NotImplementedException();
        }

        public void Translate(object o)
        {
            throw new NotImplementedException();
        }

        public string LanguageName => "None";
    }
}
