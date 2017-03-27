using System;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.TranslationTest
{
    public class TranslationTestHelper
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

            var curPath = Path.GetFullPath(".");

            while (curPath.Length > 3)
            {
                var candidate = Path.Combine(curPath, "Languages");

                if (File.Exists(Path.Combine(candidate, "english.ini")))
                {
                    return Path.GetFullPath(candidate);
                }

                curPath = Path.GetFullPath(Path.Combine(curPath, ".."));
            }

            throw new IOException("Could not find translation file folder. Current dir:" + Environment.CurrentDirectory);
        }
    }
}
