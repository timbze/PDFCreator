using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using TrueTypeFontInfo;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IFontHelper
    {
        string FindPostScriptName(string fontFamilyName);
    }

    public class FontHelper : IFontHelper
    {
        private Dictionary<string, string> _psFontDictionary;

        public string FindPostScriptName(string fontFamilyName)
        {
            if (_psFontDictionary == null)
                _psFontDictionary = BuildFontDictionary();

            if (_psFontDictionary.ContainsKey(fontFamilyName))
                return _psFontDictionary[fontFamilyName];

            return null;
        }

        public bool PostscriptFontExists(string postscriptFontName)
        {
            if (_psFontDictionary == null)
                _psFontDictionary = BuildFontDictionary();

            return _psFontDictionary.ContainsValue(postscriptFontName);
        }

        private Dictionary<string, string> BuildFontDictionary()
        {
            var fontDictionary = new Dictionary<string, string>();

            var fontsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", false);

            if (fontsKey == null)
                return fontDictionary;

            var valueNames = fontsKey.GetValueNames();
            var strFontsFolder = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName, "Fonts");

            foreach (var fontKey in valueNames)
            {
                var registryFontFileName = (string)fontsKey.GetValue(fontKey);

                if (registryFontFileName == null)
                    continue;

                var fontFile = Path.Combine(strFontsFolder, registryFontFileName);
                if (Path.GetExtension(fontFile).Equals(".ttf", StringComparison.InvariantCultureIgnoreCase) && File.Exists(fontFile))
                {
                    var trueType = new TrueType();
                    trueType.Load(Path.Combine(strFontsFolder, registryFontFileName));

                    fontDictionary[trueType.FontFamilyName] = trueType.PostScriptFontName;
                }
            }

            return fontDictionary;
        }
    }
}
