using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Mail;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.UI.Views;
using pdfforge.PDFCreator.UI.Views.Dialogs;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.UserGuide;
using SimpleInjector;

namespace pdfforge.PDFCreator.IntegrationTest.TranslationTest
{
    [TestFixture]
    internal class TranslationTest
    {
        [SetUp]
        public void SetUp()
        {
            _translationTestHelper = new TranslationTestHelper();

            var a = Assembly.GetExecutingAssembly();
            var appDir = Path.GetDirectoryName(a.CodeBase.Replace(@"file:///", ""));

            if (appDir == null)
                throw new InvalidDataException("The app dir may never be null");

            _languagePath = _translationTestHelper.FindTranslationFolder();

            Assert.True(Directory.Exists(_languagePath), "Could not find language path: " + _languagePath);

            var languageLoader = new LanguageLoader(_languagePath);
            _translations = languageLoader.GetAvailableLanguages().ToList();

            _settings = new PdfCreatorSettings(new IniStorage());
            _settings.LoadData("settings.ini");

            var assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetPdfforgeAssemblyDirectory().Returns(Path.Combine(_languagePath, ".."));

            LoggingHelper.InitConsoleLogger("PDFCreator-TranslationTest", LoggingLevel.Error);
            var settingsProvider = new DefaultSettingsProvider();
            settingsProvider.UpdateSettings(_settings);

            _translationProxy = new MappedTranslationProxy(new TranslationProxy(), _languagePath + "\\_sectionMappings.txt");

            _translationHelper = new TranslationHelper(_translationProxy, settingsProvider, assemblyHelper);
            _translationHelper.InitTranslator(_settings.ApplicationSettings.Language);

            // TODO extact stuff into separate classes, so this test only contains the actual test code
        }

        private string _languagePath;
        private List<Language> _translations;
        private PdfCreatorSettings _settings;
        private TranslationHelper _translationHelper;
        private MappedTranslationProxy _translationProxy;
        private TranslationTestHelper _translationTestHelper;


        private ICollection<string> ExtractTokens(string message)
        {
            var tokenList = new List<string>();

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
            var tokenList = new List<string>();

            var matches = Regex.Matches(message, regexPattern);
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
                    Assert.Fail("Language: {2} \r\nString: {1} \r\nmissing token '{0}' \r\nTranslation: {3}", token,
                        itemName, language.CommonName, translation);
            }

            var charsToCheck = new List<char>(new[] {'[', '{', ']', '}'});

            foreach (var c in charsToCheck)
            {
                var englishCount = english.Count(x => x == c);
                var translatedCount = translation.Count(x => x == c);

                if (englishCount != translatedCount)
                {
                    Assert.Fail(
                        "Language: {2} \r\nString: {1} \r\nnumber of brackets does not match for '{0}' \r\nEnglish: {4} \r\nTranslation: {3}",
                        c, itemName, language.CommonName, translation, english);
                }
            }
        }

        private Container BuildContainer()
        {
            var container = new Container();
            container.Options.SuppressLifestyleMismatchVerification = true;

            container.Register<ITranslator, TranslationProxy>();
            container.Register(() => _translationHelper);
            container.Register(() => new ApplicationNameProvider("PDFCreator Translation Test"));
            container.Register(() => new ButtonDisplayOptions(false, false));
            container.Register(() => new LicenseOptionProvider(false));
            container.Register<WelcomeCommand>(() => container.GetInstance<ShowWelcomeWindowCommand>());

            // use RegisterMock entension method to create NSubstitute mocks
            container.RegisterMock<IUserGuideHelper>();
            container.RegisterMock<IUserGuideLauncher>();
            container.RegisterMock<IAssemblyHelper>();
            container.RegisterMock<IVersionHelper>();
            container.RegisterMock<ILanguageProvider>();
            container.RegisterMock<IUpdateAssistant>();
            container.RegisterMock<IFile>();
            container.RegisterMock<IDirectory>();
            container.RegisterMock<IPath>();
            container.RegisterMock<IProcessStarter>();
            container.RegisterMock<IUacAssistant>();
            container.RegisterMock<IPrinterProvider>();
            container.RegisterMock<IInteractionInvoker>();
            container.RegisterMock<IPrinterHelper>();
            container.RegisterMock<ITestPageHelper>();
            container.RegisterMock<ILicenseServerHelper>();
            container.RegisterMock<IWelcomeSettingsHelper>();
            container.RegisterMock<IPlusHintHelper>();
            container.RegisterMock<IFileConversionHandler>();
            container.RegisterMock<IJobInfoQueue>();
            container.RegisterMock<IFontHelper>();
            container.RegisterMock<IEmailClientFactory>();
            container.RegisterMock<ISmtpTest>();
            container.RegisterMock<IPdfArchitectCheck>();
            container.RegisterMock<ISoundPlayer>();
            container.RegisterMock<ITempFolderProvider>();
            container.RegisterMock<ISpoolerProvider>();
            container.RegisterMock<IProfileChecker>();
            container.RegisterMock<ISignaturePasswordCheck>();
            container.RegisterMock<IScriptActionHelper>();
            container.RegisterMock<IOpenFileInteractionHelper>();
            container.RegisterMock<IClientTestEmail>();
            container.RegisterMock<ISystemPrinterProvider>();
            container.RegisterMock<IPrinterActionsAssistant>();
            container.RegisterMock<ILicenseChecker>();
            container.RegisterMock<IOsHelper>();
            container.RegisterMock<IDispatcher>();
            container.RegisterMock<IIniSettingsAssistant>();
            container.RegisterMock<IJobInfoManager>();
            container.RegisterMock<ISettingsManager>();
            container.RegisterMock<IActivationHelper>();
            container.RegisterMock<IDropboxService>();
            container.RegisterMock<IWinInetHelper>();
            container.Register(() => new DropboxAppData("", ""));

            container.Register(() => ViewCustomization.DefaultCustomization);

            var settingsHelperRegistration =
                Lifestyle.Singleton.CreateRegistration(() => Substitute.For<ISettingsProvider>(), container);
            container.AddRegistration(typeof (ISettingsProvider), settingsHelperRegistration);

            // Make sure that SettingsHelper will not be instantiated
            container.Register<SettingsProvider>(
                () =>
                {
                    throw new InvalidOperationException(
                        "SettingsHelper must not be used directly. Use ISettingsHelper instead.");
                });

            return container;
        }

        private void TestActionControls(ITranslator trans, Language lang, Container container)
        {
            var type = typeof (UserControl);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t.Namespace.StartsWith("pdfforge") && !t.IsAbstract).ToList();

            var reflectionTranslator = new ReflectionTranslator(trans);

            foreach (var t in types)
            {
                var typeName = t.FullName;
                try
                {
                    if (!t.IsAbstract && !(t == typeof (ActionControl)))
                    {
                        var userControl = container.GetInstance(t);

                        reflectionTranslator.Translate(userControl);

                        if (reflectionTranslator.TranslationErrors.Any())
                        {
                            var details = reflectionTranslator.TranslationErrors.First();
                            throw new Exception($"\r\nSection: {details.Description}\r\nItem:{details.Item}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error while translating " + t.Name + " with " + lang.FileName, ex);
                }
            }
        }

        private void MaybeAddType(IList<Type> types, Type type)
        {
            if (!types.Contains(type))
                types.Add(type);
        }

        private void TestWindows(ITranslator trans, Language lang, Container container)
        {
            var type = typeof (Window);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t.Namespace.StartsWith("pdfforge")).ToList();

            MaybeAddType(types, typeof (PasswordWindow));

            var reflectionTranslator = new ReflectionTranslator(trans);

            foreach (var t in types)
            {
                try
                {
                    if (!t.IsAbstract)
                    {
                        var window = container.GetInstance(t);

                        reflectionTranslator.Translate(window);

                        if (reflectionTranslator.TranslationErrors.Any())
                        {
                            var details = reflectionTranslator.TranslationErrors.First();
                            throw new Exception($"\r\nSection: {details.Description}\r\nItem:{details.Item}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error while translating " + t.Name + " with " + lang.FileName, ex);
                }
            }
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
                        new Uri("PDFCreator.Views;component/Resources/AllResources.xaml",
                            UriKind.Relative)) as ResourceDictionary);
            }
        }

        private Language GetLanguage(string filename)
        {
            return _translations.First(x => x.FileName.EndsWith(filename, StringComparison.InvariantCultureIgnoreCase));
        }

        [Test, TestCaseSource(typeof (TranslationTestCaseSource), nameof(TranslationTestCaseSource.TestCases))]
        public void AllLanguages_ContainSameTokensAsEnglish(string translation)
        {
            // These translations contain something that looks like a token, but it is okay if they do not match
            var exceptions = new[]
            {
                "pdfforge.PDFCreator.UI.Views.ActionControls.UserTokenActionControl\\DescriptionText.Text"
            };

            var englishLanguage =
                _translations.First(x => x.CommonName.Equals("English", StringComparison.InvariantCultureIgnoreCase));
            var englishTranslationData = Data.CreateDataStorage();
            var iniStorage = new IniStorage();
            iniStorage.SetData(englishTranslationData);
            iniStorage.ReadData(Path.Combine(_languagePath, englishLanguage.FileName), true);

            var lang = GetLanguage(translation);

            var translationData = Data.CreateDataStorage();
            var translatedIni = new IniStorage();
            translatedIni.SetData(translationData);
            translatedIni.ReadData(Path.Combine(_languagePath, lang.FileName), true);

            ITranslator trans = new BasicTranslator(lang.CommonName, translationData);

            foreach (var section in englishTranslationData.GetSubSections(""))
            {
                if (section.StartsWith("General"))
                    continue;

                var values = englishTranslationData.GetValues(section);

                foreach (var keyValuePair in values)
                {
                    if (exceptions.Contains(section + keyValuePair.Key))
                        continue;

                    var translatedString = trans.GetTranslation(section.TrimEnd('\\'), keyValuePair.Key);

                    if (translatedString != "")
                        ValidateSingleTranslation(keyValuePair.Value, translatedString, lang, section + keyValuePair.Key);
                }
            }
        }

        [Test, TestCaseSource(typeof (TranslationTestCaseSource), nameof(TranslationTestCaseSource.TestCases))]
        [RequiresSTA]
        public void AllWindows_WhenTranslatedWithAllLanguages_DoNotThrowExceptions(string translation)
        {
            var lang = GetLanguage(translation);

            var translationData = Data.CreateDataStorage();
            var iniStorage = new IniStorage();
            iniStorage.SetData(translationData);
            var translationFile = Path.Combine(_languagePath, lang.FileName);
            iniStorage.ReadData(translationFile, true);

            var exceptionTranslator = new ExceptionTranslator(_translationProxy);
            _translationProxy.Translator = new BasicTranslator(lang.CommonName, translationData);
            _translationProxy.LoadOldSectionNames(translationFile);

            // Do some work to include all resources as we do not have our WPF app context here
            EnsureApplicationResources();

            var container = BuildContainer();

            LoggingHelper.InitConsoleLogger("TranslationTest", LoggingLevel.Off);
            var settingsProvider = container.GetInstance<ISettingsProvider>();
            settingsProvider.Settings.Returns(new PdfCreatorSettings(new IniStorage()));
            settingsProvider.GpoSettings.Returns(new GpoSettingsDefaults());

            // Everything with the type "Window" is tested automatically. If special parameters are needed for a type, they can be declared here

            TestWindows(_translationProxy, lang, container);
            TestActionControls(_translationProxy, lang, container);

            Assert.IsEmpty(exceptionTranslator.TranslationErrors, "There were errors while translating the forms");
        }

        [Test]
        public void TestEnumTranslationsInEnglish()
        {
            var lang = _translations.FirstOrDefault(l => l.CommonName.Equals("English"));
            Assert.IsNotNull(lang, "Could not load english translation");
            ITranslator trans = new ExceptionTranslator(new BasicTranslator(Path.Combine(_languagePath, lang.FileName)));

            var enumsWithoutTranslation = new List<Type>();
            enumsWithoutTranslation.Add(typeof (EncryptionLevel)); ////Associated radio buttons get translated manually
            enumsWithoutTranslation.Add(typeof (LoggingLevel));

            EnsureApplicationResources();

            //Get assembly which defines the settings
            var assembly = Assembly.GetAssembly(typeof (PdfCreatorSettings));
            //Get all enumTypes in "pdfforge.PDFCreator.Core.Settings.Enums"
            var enumTypes =
                assembly.GetTypes()
                    .Where(
                        t =>
                            string.Equals(t.Namespace, "pdfforge.PDFCreator.Core.Settings.Enums",
                                StringComparison.Ordinal));
            //Remove all enumtypes without translation
            enumTypes = enumTypes.Where(e => !enumsWithoutTranslation.Contains(e));

            foreach (var e in enumTypes)
            {
                var values = Enum.GetValues(e);
                foreach (var v in values)
                {
                    var enumValue = e.Name + "." + v;
                    Assert.IsNotNullOrEmpty(trans.GetTranslation("Enums", enumValue),
                        enumValue + " has no translation in " + lang.CommonName);
                }
            }
        }

        [Test, Ignore("This can be run manually to update translation mappings")]
        public void UpdateTranslationsWithMappings()
        {
            var translations = Directory.EnumerateFiles(_languagePath, "*.ini");

            foreach (var translationFile in translations)
            {
                var translationContents = new StringBuilder(File.ReadAllText(translationFile));

                foreach (var oldSection in _translationProxy.OldSectionReverseMapping.Keys)
                {
                    var newSection = _translationProxy.OldSectionReverseMapping[oldSection];

                    translationContents.Replace($"[{oldSection}]", $"[{newSection}]");
                }

                File.WriteAllText(translationFile, translationContents.ToString());
            }
        }

        [TestCaseSource(nameof(ErrorCodes))]
        public void AllErrorCodes_ArePresentInTranslation(ErrorCode errorCode)
        {
            // Store the english translator for other tests...
            if (_englishTranslator == null)
                _englishTranslator = new BasicTranslator(Path.Combine(_translationTestHelper.FindTranslationFolder(), "english.ini"));

            var section = "ErrorCodes";

            int exitCode = (int) errorCode;
            var defaultValue = StringValueAttribute.GetValue(errorCode);
            Assert.AreEqual(defaultValue, _englishTranslator.GetTranslation(section, exitCode.ToString()), $"The value for {errorCode} ({exitCode}) is not set!\r\nAdd:\r\n{exitCode}={defaultValue}");
        }

        private static readonly IEnumerable<ErrorCode> ErrorCodes = Enum.GetValues(typeof(ErrorCode)).Cast<ErrorCode>();
        private BasicTranslator _englishTranslator;
    }

    internal class ExceptionTranslator : TranslatorBase
    {
        private readonly ITranslator _translator;

        public readonly IList<string> TranslationErrors = new List<string>();

        public ExceptionTranslator(ITranslator translator)
        {
            _translator = translator;
        }

        public override string LanguageName => _translator.LanguageName;

        protected override string GetRawTranslation(string section, string item)
        {
            var translation = _translator.GetTranslation(section, item);

            if (string.IsNullOrWhiteSpace(translation))
                TranslationErrors.Add(
                    $"Error while translating {item} in {section} (Language: {_translator.LanguageName})");

            return translation;
        }

        public override IList<string> GetKeysForSection(string section)
        {
            return _translator.GetKeysForSection(section);
        }
    }

    public class TranslationTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                var translationTestHelper = new TranslationTestHelper();
                var translationFolder = translationTestHelper.FindTranslationFolder();

                foreach (var translation in Directory.EnumerateFiles(translationFolder, "*.ini"))
                {
                    var filename = Path.GetFileName(translation);
                    yield return new TestCaseData(filename)
                        .SetName(filename)
                        .SetDescription($"Test translation for {filename}");
                }
            }
        }
    }

    internal static class ContainerMockExtension
    {
        public static void RegisterMock<T>(this Container container) where T : class
        {
            container.Register(() => Substitute.For<T>());
        }
    }
}