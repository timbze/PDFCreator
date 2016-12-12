using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Services.Translation;

namespace pdfforge.PDFCreator.IntegrationTest.TranslationTest
{
    internal class MappedTranslationProxy : TranslationProxy
    {
        private readonly IDictionary<string, string> _oldSectionMapping = new Dictionary<string, string>();
        private string _mappingFile;

        private IList<string> _sectionNames;

        public MappedTranslationProxy(ITranslator translator, string mappingFile)
        {
            Translator = translator;
            MappingFile = mappingFile;
        }

        public IDictionary<string, string> OldSectionReverseMapping { get; } = new Dictionary<string, string>();

        public string MappingFile
        {
            get { return _mappingFile; }
            set
            {
                _mappingFile = value;
                LoadMappings(value);
            }
        }

        protected override string GetRawTranslation(string section, string item)
        {
            if (Translator == null)
                return "";

            var translation = Translator.GetTranslation(section, item);

            if (string.IsNullOrWhiteSpace(translation) && _oldSectionMapping.ContainsKey(section))
            {
                translation = Translator.GetTranslation(_oldSectionMapping[section], item);
            }

            return translation;
        }

        public override IList<string> GetKeysForSection(string section)
        {
            if (Translator == null)
                return new List<string>();

            var keys = Translator.GetKeysForSection(section);

            if (keys.Count == 0)
            {
                var oldName = GetOldSectionName(section);
                if (!string.IsNullOrWhiteSpace(oldName))
                {
                    keys = Translator.GetKeysForSection(oldName);
                }
            }

            return keys;
        }

        public void LoadOldSectionNames(string languageFile)
        {
            var iniReader = new IniStorage(Encoding.UTF8);
            var data = Data.CreateDataStorage();
            iniReader.SetData(data);

            iniReader.ReadData(languageFile);

            _sectionNames = data.GetSections()
                .Where(section => section.Contains("."))
                .Select(section => section.TrimEnd('\\'))
                .ToList();
        }

        private string GetOldSectionName(string sectionName)
        {
            if (!sectionName.Contains("."))
                return null;

            if (_oldSectionMapping.ContainsKey(sectionName))
                return _oldSectionMapping[sectionName];

            var className = sectionName.Split('.').Last();

            var oldSectionName = _sectionNames.FirstOrDefault(x => x.EndsWith("." + className));

            if (!string.IsNullOrWhiteSpace(oldSectionName))
            {
                AddMapping(oldSectionName, sectionName);
            }

            if (!string.IsNullOrWhiteSpace(MappingFile))
                SaveMappings(MappingFile);

            return oldSectionName;
        }

        private void AddMapping(string oldSection, string newSection)
        {
            _oldSectionMapping[newSection] = oldSection;
            OldSectionReverseMapping[oldSection] = newSection;
        }

        private void SaveMappings(string mappingFile)
        {
            var mappings = new List<string>();

            foreach (var oldSection in OldSectionReverseMapping.Keys)
            {
                var newSection = OldSectionReverseMapping[oldSection];
                mappings.Add($"{oldSection}=>{newSection}");
            }

            mappings.Sort();

            File.WriteAllText(mappingFile, string.Join("\r\n", mappings));
        }

        private void LoadMappings(string mappingFile)
        {
            if (!File.Exists(mappingFile))
                return;

            var lines = File.ReadAllLines(mappingFile).Where(line => line.Contains("=>"));
            foreach (var line in lines)
            {
                var parts = line.Split(new[] {"=>"}, StringSplitOptions.None);
                var oldName = parts[0];
                var newName = parts[1];
                AddMapping(oldName, newName);
            }
        }
    }
}