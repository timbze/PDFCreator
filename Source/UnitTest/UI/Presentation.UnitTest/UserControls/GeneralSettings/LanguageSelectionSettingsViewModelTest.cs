using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.Generic;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class LanguageSelectionSettingsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _languageProvider = Substitute.For<ILanguageProvider>();
            _gpoSettings = Substitute.For<IGpoSettings>();
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _translationHelper = Substitute.For<ITranslationHelper>();
            _currentSettingsProvider.Settings.Returns(new PdfCreatorSettings(new IniStorage()));
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ILanguageProvider _languageProvider;

        private ITranslationUpdater _translationUpdater;
        private IGpoSettings _gpoSettings;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private ITranslationHelper _translationHelper;

        private LanguageSelectionSettingsViewModel BuildViewModel()
        {
            return new LanguageSelectionSettingsViewModel(_gpoSettings, _currentSettingsProvider, _languageProvider, _translationHelper, _translationUpdater);
        }

        [Test]
        public void AddLanguageToProvider_RequestLanguagesInViewModel_TestFirstLanugageWithCreatedLanguage()
        {
            IList<Language> languages = new List<Language>();
            var language = new Language();
            languages.Add(language);

            _languageProvider.GetAvailableLanguages().Returns(languages);

            var viewModel = BuildViewModel();

            var viewModelLanguages = viewModel.Languages;
            Assert.AreEqual(language, viewModelLanguages.First());
        }

        [Test]
        public void ApplicationIsNotSet_RequestCurrentLanguage_ReturnsNull()
        {
            _currentSettingsProvider.Settings.Returns(x => null);
            var viewModel = BuildViewModel();
            Assert.IsNull(viewModel.CurrentLanguage);
        }

        [Test]
        public void ApplicationSettingsIsNull_RequestLangaugeIsEnabled_GetTrue()
        {
            _currentSettingsProvider.Settings.Returns(info => null);
            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.LanguageIsEnabled);
        }

        [Test]
        public void ApplicationSettingsIsSetAndGPOSettingsIsNull_RequestLangaugeIsEnabled_GetTrue()
        {
            _gpoSettings = null;

            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.LanguageIsEnabled);
        }

        [Test]
        public void ApplicationSettingsIsSetAndGPOSettingsIsSetAndGPOLanguage_RequestLangaugeIsEnabled_GetFalse()
        {
            var viewModel = BuildViewModel();
            Assert.IsFalse(viewModel.LanguageIsEnabled);
        }

        [Test]
        public void ApplicationSettingsIsSetAndGPOSettingsIsSetAndNotGPOLanguage_RequestLangaugeIsEnabled_GetTrue()
        {
            _gpoSettings.Language.Returns(x => null);
            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.LanguageIsEnabled);
        }

        [Test]
        public void ExecutePreviewTranslation_TranslationHelperGetsCalled()
        {
            IList<Language> languages = new List<Language>();
            var language = new Language();
            language.CommonName = "XX";
            language.Iso2 = "XX";
            languages.Add(language);
            _languageProvider.GetAvailableLanguages().Returns(languages);

            _gpoSettings = null;

            var viewModel = BuildViewModel();

            viewModel.CurrentLanguage = "XX";
            var setTempTrans = false;
            var translateProfileList = false;

            _translationHelper.When(helper => helper.SetTemporaryTranslation(language)).Do(info => setTempTrans = true);
            _translationHelper.When(helper => helper.TranslateProfileList(Arg.Any<IList<ConversionProfile>>())).Do(info => translateProfileList = true);
            viewModel.PreviewTranslationCommand.Execute(null);

            Assert.IsTrue(setTempTrans);
            Assert.IsTrue(translateProfileList);
        }

        [Test]
        public void GetCurrentLangauge_LanguageIsSetInApplicationSettings()
        {
            var language = "notTheDefault";

            var viewModel = BuildViewModel();

            viewModel.CurrentLanguage = language;
            Assert.AreEqual(viewModel.ApplicationSettings.Language, language);
        }

        [Test]
        public void GPOSettingsIsNotSet_RequestCurrentLanguage_GetApplicationLanguage()
        {
            _gpoSettings = null;
            var viewModel = BuildViewModel();
            Assert.AreEqual(_currentSettingsProvider.Settings.ApplicationSettings.Language, viewModel.CurrentLanguage);
        }

        [Test]
        public void GPOSettingsIsSet_RequestCurrentLanguage_GPOLanguage()
        {
            var returnThis = "notTheDefault";
            _gpoSettings.Language.Returns(returnThis);
            var viewModel = BuildViewModel();
            Assert.AreEqual(returnThis, viewModel.CurrentLanguage);
        }

        [Test]
        public void GPOSettingsLanguageIsNull_RequestCurrentLanguage_GetApplicationLanguage()
        {
            _gpoSettings.Language.Returns(x => null);
            var viewModel = BuildViewModel();
            Assert.AreEqual(_currentSettingsProvider.Settings.ApplicationSettings.Language, viewModel.CurrentLanguage);
        }
    }
}
