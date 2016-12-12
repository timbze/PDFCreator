using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using pdfforge.DataStorage;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    /// <summary>
    ///     TranslationUtil provides functionality that is used in conjunction with the DynamicTranslator classes.
    /// </summary>
    public class TranslationHelper : ILanguageProvider
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly TranslationProxy _translationProxy;
        private LanguageLoader _languageLoader;
        private ITranslator _tmpTranslator;

        public TranslationHelper(TranslationProxy translationProxy, ISettingsProvider settingsProvider, IAssemblyHelper assemblyHelper)
        {
            _translationProxy = translationProxy;

            _settingsProvider = settingsProvider;
            _settingsProvider.LanguageChanged += SettingsProviderOnLanguageChanged;
            _assemblyHelper = assemblyHelper;
        }

        private LanguageLoader LanguageLoader
        {
            get
            {
                if (_languageLoader == null)
                    _languageLoader = BuildLanguageLoader();

                return _languageLoader;
            }
            set { _languageLoader = value; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public List<string> PossibleLanguagePaths { get; set; } = new List<string> {"Languages", @"..\..\..\..\..\Languages"};

        public IEnumerable<Language> GetAvailableLanguages()
        {
            return LanguageLoader.GetAvailableLanguages();
        }

        public bool HasTranslation(string language)
        {
            return LanguageLoader.GetTranslationFileIfExists(language) != null;
        }

        public Language FindBestLanguage(CultureInfo culture)
        {
            return LanguageLoader.FindBestLanguage(culture);
        }

        private void SettingsProviderOnLanguageChanged(object sender, EventArgs eventArgs)
        {
            var applicationLanguage = _settingsProvider.GetApplicationLanguage();
            InitTranslator(applicationLanguage);
            TranslateProfileList(_settingsProvider.Settings.ConversionProfiles);
        }

        /// <summary>
        ///     Initialize the Translator for later use in the application
        /// </summary>
        /// <param name="languageName">Language to use for initialization</param>
        public void InitTranslator(string languageName)
        {
            LanguageLoader = BuildLanguageLoader();

            if (string.IsNullOrEmpty(languageName))
                languageName = "english";

            var translationFile = LanguageLoader.GetTranslationFile(languageName);

            var translator = BuildLanguageTranslator(translationFile);
            var fallback = BuildLanguageTranslator(Path.Combine(LanguageLoader.TranslationFolder, "english.ini"));

            _translationProxy.Translator = new FallbackTranslator(translator, fallback);
        }

        /// <summary>
        ///     Initialize an empty translator (i.e. for tests)
        /// </summary>
        public void InitEmptyTranslator()
        {
            _languageLoader = BuildLanguageLoader();
        }

        private ITranslator BuildLanguageTranslator(string translationFile)
        {
            if ((translationFile == null) || !File.Exists(translationFile))
                return new BasicTranslator("empty", Data.CreateDataStorage());

            return new BasicTranslator(translationFile);
        }

        private LanguageLoader BuildLanguageLoader()
        {
            var appDir = _assemblyHelper.GetPdfforgeAssemblyDirectory();

            var translationPathCandidates = PossibleLanguagePaths.Select(path => Path.Combine(appDir, path)).ToArray();

            return new LanguageLoader(translationPathCandidates);
        }

        /// <summary>
        ///     Temporarily sets a translation while storing the old translator for later use. Use RevertTemporaryTranslation to
        ///     revert to the initial translator.
        /// </summary>
        /// <param name="language">The language definition to use</param>
        /// <returns>true, if the translation was successfully loaded</returns>
        public bool SetTemporaryTranslation(Language language)
        {
            var languageFile = Path.Combine(LanguageLoader.TranslationFolder, language.FileName);

            if (!File.Exists(languageFile))
                return false;

            if (_tmpTranslator == null)
                _tmpTranslator = _translationProxy.Translator;

            _translationProxy.Translator = new BasicTranslator(languageFile);

            return true;
        }

        /// <summary>
        ///     Reverts a temporarily set translation to it's original. If no temporary translation has been set, nothing will be
        ///     reverted.
        /// </summary>
        public void RevertTemporaryTranslation()
        {
            if (_tmpTranslator != null)
            {
                _translationProxy.Translator = _tmpTranslator;
                _tmpTranslator = null;
            }
        }

        /// <summary>
        ///     Translates a profile list by searching for predefined translations based on their GUID and apply the translated
        ///     name to them
        /// </summary>
        /// <param name="profiles">The profile list</param>
        public void TranslateProfileList(IList<ConversionProfile> profiles)
        {
            foreach (var p in profiles)
                try
                {
                    var translation = _translationProxy.GetTranslation("ProfileNameByGuid", p.Guid);
                    if (!string.IsNullOrEmpty(translation))
                        p.Name = translation;
                }
                catch (ArgumentException)
                {
                    //do nothing, profile must not be renamed 
                }
        }
    }
}
