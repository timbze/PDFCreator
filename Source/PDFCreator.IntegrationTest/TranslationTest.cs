using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.DynamicTranslator;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Helper.Logging;
using pdfforge.PDFCreator.Shared.Views;
using pdfforge.PDFCreator.Shared.Views.ActionControls;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Views;

namespace PDFCreator.IntegrationTest
{
    [TestFixture]
    class TranslationTest
    {
        string _languagePath;
        List<Language> _translations;
        PdfCreatorSettings _settings;

        [SetUp]
        public void SetUp()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string appDir = Path.GetDirectoryName(a.CodeBase.Replace(@"file:///", ""));

            if (appDir == null)
                throw new InvalidDataException("The app dir may never be null");

            _languagePath = FindTranslationFolder();

            Assert.True(Directory.Exists(_languagePath), "Could not find language path: " + _languagePath);

            _translations = Translator.FindTranslations(_languagePath);

            _settings = new PdfCreatorSettings(new IniStorage());
            _settings.LoadData("settings.ini");

            IAssemblyHelper assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetCurrentAssemblyDirectory().Returns(Path.Combine(_languagePath, ".."));

            LoggingHelper.InitConsoleLogger("PDFCreator-TranslationTest", LoggingLevel.Error);
            TranslationHelper.Instance.InitTranslator(_settings.ApplicationSettings.Language, assemblyHelper);
        }

        private static string FindTranslationFolder()
        {
            var candidates = new[] { @"..\..\..\PDFCreator\Languages", @"..\PDFCreator\Languages", @"PDFCreator\Languages", @"Languages", @"Source\PDFCreator\Languages", @"..\..\Source\PDFCreator\Languages" };

            foreach (string dir in candidates)
            {
                if (File.Exists(Path.Combine(dir, "english.ini")))
                {
                    return dir;
                }
            }

            throw new IOException("Could not find test file folder");
        }

        [Test]
        [RequiresSTA]
        public void AllWindows_WhenTranslatedWithAllLanguages_DoNotThrowExceptions()
        {
            foreach (Language lang in _translations)
            {
                TestLanguage(lang);
            }
        }

        [Test]
        public void AllLanguages_ContainSameTokensAsEnglish()
        {
            var englishLanguage = _translations.First(x => x.CommonName.Equals("English", StringComparison.InvariantCultureIgnoreCase));
            Data englishTranslationData = Data.CreateDataStorage();
            IniStorage iniStorage = new IniStorage();
            iniStorage.SetData(englishTranslationData);
            iniStorage.ReadData(Path.Combine(_languagePath, englishLanguage.FileName), true);

            foreach (Language lang in _translations)
            {
                if (lang != englishLanguage)
                {
                    TestTokensInTranslation(englishTranslationData, lang);
                }
            }
        }

        private void TestTokensInTranslation(Data englishTranslationData, Language language)
        {
            Data translationData = Data.CreateDataStorage();
            IniStorage iniStorage = new IniStorage();
            iniStorage.SetData(translationData);
            iniStorage.ReadData(Path.Combine(_languagePath, language.FileName), true);

            Translator trans = new BasicTranslator(language.CommonName, translationData);

            foreach (var section in englishTranslationData.GetSubSections(""))
            {
                if (section.StartsWith("General"))
                    continue;

                var values = englishTranslationData.GetValues(section);

                foreach (var keyValuePair in values)
                {
                    string translation = trans.GetRawTranslation(section.TrimEnd('\\'), keyValuePair.Key);

                    if (translation != "")
                        ValidateSingleTranslation(keyValuePair.Value, translation, language, section + keyValuePair.Key);
                }
            }
        }

        private ICollection<string> ExtractTokens(string message)
        {
            List<string> tokenList = new List<string>();

            // curly brackets, i.e. {0}, {1:3f}
            tokenList.AddRange(ExtractTokens(message, @"(\{.*?\})"));
            // square brackets, i.e. [Name]
            tokenList.AddRange(ExtractTokens(message, @"(\[.*?\])"));
            // number tokens, i.e. %2
            tokenList.AddRange(ExtractTokens(message, @"(%\d)"));

            return tokenList;
        }

        private ICollection<string> ExtractTokens(string message, string regexPattern)
        {
            List<string> tokenList = new List<string>();

            MatchCollection matches = Regex.Matches(message, regexPattern);
            foreach (Match match in matches)
            {
                tokenList.Add(match.Groups[1].Value);
            }

            return tokenList;
        }

        private void ValidateSingleTranslation(string english, string translation, Language language, string itemName)
        {
            var tokens = ExtractTokens(english);

            foreach (var token in tokens)
            {
                if (!translation.Contains(token))
                    Assert.Fail("Language: {2} \r\nString: {1} \r\nmissing token '{0}' \r\nTranslation: {3}", token, itemName, language.CommonName, translation);
            }

            var charsToCheck = new List<char>(new []{'[', '{', ']', '}' });

            foreach (char c in charsToCheck)
            {
                var englishCount = english.Count(x => x == c);
                var translatedCount = translation.Count(x => x == c);

                if (englishCount != translatedCount)
                {
                    Assert.Fail("Language: {2} \r\nString: {1} \r\nnumber of brackets does not match for '{0}' \r\nEnglish: {4} \r\nTranslation: {3}", c, itemName, language.CommonName, translation, english);
                }
            }
            
        }

        private void TestLanguage(Language lang)
        {
            Data translationData = Data.CreateDataStorage();
            IniStorage iniStorage = new IniStorage();
            iniStorage.SetData(translationData);
            iniStorage.ReadData(Path.Combine(_languagePath, lang.FileName), true);

            Translator trans = new BasicTranslator(lang.CommonName, translationData);

            trans.TranslationError += trans_TranslationError;

            // Do some work to include all resources as we do not have our WPF app context here
            EnsureApplicationResources();

            var args = new Dictionary<Type, object[]>();

            // Everything with the type "Window" is tested automatically. If special parameters are needed for a type, they can be declared here
            args.Add(typeof(EditEmailTextWindow), new object[] { false });
            args.Add(typeof(EncryptionPasswordsWindow), new object[] {EncryptionPasswordMiddleButton.Remove, true, true});
            args.Add(typeof(FtpPasswordWindow), new object[] { FtpPasswordMiddleButton.Remove });
            args.Add(typeof(MessageWindow), new object[] { "", "", MessageWindowButtons.YesLaterNo, MessageWindowIcon.PDFCreator });
            args.Add(typeof(ProfileSettingsWindow), new object[] { new PdfCreatorSettings(new IniStorage()), new GpoSettings() });
            args.Add(typeof(RecommendPdfArchitectWindow), new object[] { false });
            args.Add(typeof(SignaturePasswordWindow), new object[] { PasswordMiddleButton.Skip, null });
            args.Add(typeof(SmtpPasswordWindow), new object[] { SmtpPasswordMiddleButton.Remove });
            args.Add(typeof(UpdateDownloadWindow), new object[] { null });

            TestWindows(trans, lang, args);
            TestActionControls(trans, lang);

            Assert.IsEmpty(trans.TranslationErrors, "There were errors while translating the forms");
        }

        private void TestActionControls(Translator trans, Language lang)
        {
            var type = typeof(UserControl);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t.Namespace.StartsWith("pdfforge")).ToList();

            AttachmentActionControl x = new AttachmentActionControl();
            UserControl y = x;

            foreach (var t in types)
            {
                try
                {
                    if (!t.IsAbstract)
                    {
                        var userControl = (UserControl) Activator.CreateInstance(t);
                        trans.Translate(userControl);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error while translating " + t.Name + " with " + lang.FileName, ex);
                }
            }
        }

        private void TestWindows(Translator trans, Language lang, Dictionary<Type, object[]> argsDictionary)
        {
            var type = typeof(Window);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t.Namespace.StartsWith("pdfforge")).ToList();

            foreach (var t in types)
            {
                try
                {
                    if (!t.IsAbstract)
                    {
                        Window window;

                        if (argsDictionary.ContainsKey(t))
                        {
                            window = (Window) Activator.CreateInstance(t, argsDictionary[t]);
                        }
                        else
                        {
                            window = (Window) Activator.CreateInstance(t);
                        }
                        trans.Translate(window);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error while translating " + t.Name + " with " + lang.FileName, ex);
                }
            }
        }

        void trans_TranslationError(object sender, TranslationErrorEventArgs e)
        {
            throw new ArgumentException(String.Format("Error while translating {0} in {1} (Language: {2})", e.Item, e.Description, e.Language));
        }

        private static void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                // create the Application object
                var app = new Application();

                // merge in your application resources
                app.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(
                        new Uri("PDFCreator;component/Resources/AllResources.xaml",
                        UriKind.Relative)) as ResourceDictionary);
            }
        }

        [Test]
        public void TestEnumTranslationsInEnglish()
        {
            Language lang = _translations.FirstOrDefault(l => l.CommonName.Equals("English"));
            Assert.IsNotNull(lang, "Could not load english translation");
            Translator trans = new BasicTranslator(Path.Combine(_languagePath, lang.FileName));
            
            var enumsWithoutTranslation = new List<Type>();
            enumsWithoutTranslation.Add(typeof(ApiProvider));
            enumsWithoutTranslation.Add(typeof(EncryptionLevel)); ////Associated radio buttons get translated manually
            enumsWithoutTranslation.Add(typeof(LoggingLevel));

            EnsureApplicationResources();

            //Get assembly which defines the settings
            var assembly = Assembly.GetAssembly(typeof (PdfCreatorSettings));
            //Get all enumTypes in "pdfforge.PDFCreator.Core.Settings.Enums"
            var enumTypes = assembly.GetTypes().Where(t => String.Equals(t.Namespace, "pdfforge.PDFCreator.Core.Settings.Enums", StringComparison.Ordinal));
            //Remove all enumtypes without translation
            enumTypes = enumTypes.Where(e => !enumsWithoutTranslation.Contains(e));

            foreach (var e in enumTypes)
            {
                var values = Enum.GetValues(e);
                foreach (var v in values)
                {
                    string enumValue = e.Name +"."+ v;
                    Assert.IsNotNullOrEmpty(trans.GetTranslation("Enums", enumValue), enumValue + " has no translation in " + lang.CommonName);
                }
            }
        }
    }
}