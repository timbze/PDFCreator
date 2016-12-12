using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pdfforge.PDFCreator.IntegrationTest.TranslationTest
{
    class TranslationTestHelper
    {
        public string FindTranslationFolder()
        {
            var candidates = new[]
            {
                @"Languages", @"..\..\..\..\Languages", @"Source\Languages", @"..\..\Source\Languages", @"..\..\..\Languages"
            };

            foreach (var dir in candidates)
            {
                if (File.Exists(Path.Combine(dir, "english.ini")))
                {
                    return Path.GetFullPath(dir);
                }
            }

            throw new IOException("Could not find translation file folder. Current dir:" + Environment.CurrentDirectory);
        }
    }
}
