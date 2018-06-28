using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class ExportSettingsViewModelTest
    {
        private IInteractionInvoker _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IGpoSettings _gpoSettings;
        private ISettingsProvider _simpleSettingsProvider;
        private IIniSettingsAssistant _iniSettingsAssitant;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();

            IStorage storage = Substitute.For<IStorage>();
            var pdfCreatorSettings = new PdfCreatorSettings(storage);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(pdfCreatorSettings);

            _gpoSettings = Substitute.For<IGpoSettings>();

            _simpleSettingsProvider = Substitute.For<ISettingsProvider>();
            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_simpleSettingsProvider);

            _iniSettingsAssitant = Substitute.For<IIniSettingsAssistant>();

            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        public ExportSettingsViewModel BuildModel()
        {
            return new ExportSettingsViewModel(_settingsManager, _translationUpdater, _iniSettingsAssitant, _currentSettingsProvider, _gpoSettings);
        }

        [Test]
        public void GetProperties_AllPropertiesAreSet()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.LoadIniSettingsCommand);
            Assert.NotNull(viewModel.SaveIniSettingsCommand);
        }

        [Test]
        public void GPOSettingsNotSet_GetProfileManagementIsEnabled_IsTrue()
        {
            _gpoSettings = null;

            var viewModel = BuildModel();
            Assert.True(viewModel.ProfileManagementIsEnabled);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GPOSettingsSet_GetProfileManagementIsEnabled_InvertedGpoSettingsDisabledProfileManager(bool disabledProfileManagementState)
        {
            _gpoSettings.DisableProfileManagement.Returns(disabledProfileManagementState);

            var viewModel = BuildModel();

            Assert.AreEqual(viewModel.ProfileManagementIsEnabled, !disabledProfileManagementState);
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _invoker
                .When(invoker => invoker.Invoke(Arg.Any<MessageInteraction>()))
                .Do(messageInteraction => action(messageInteraction.Arg<MessageInteraction>()));
        }

        [Test]
        public void LoadSettings_SettingsAreSetAndEventIsCalled()
        {
            IStorage storage = Substitute.For<IStorage>();
            var settings = new PdfCreatorSettings(storage);

            settings.ApplicationSettings.PrimaryPrinter = "primaryPrinter";
            _simpleSettingsProvider.Settings.Returns(settings);
            _iniSettingsAssitant.LoadIniSettings().Returns(true);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.Yes);

            var viewModel = BuildModel();

            var wasSettingsLoadedCalled = false;
            viewModel.SettingsLoaded += (sender, args) => wasSettingsLoadedCalled = true;

            viewModel.LoadIniSettingsCommand.Execute(null);

            Assert.IsTrue(wasSettingsLoadedCalled);
        }

        [Test]
        public void LoadSettings_SettingsAreSetNoError()
        {
            IStorage storage = Substitute.For<IStorage>();
            var settings = new PdfCreatorSettings(storage);

            settings.ApplicationSettings.PrimaryPrinter = "primaryPrinter";
            _simpleSettingsProvider.Settings.Returns(settings);
            _iniSettingsAssitant.LoadIniSettings().Returns(true);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.Yes);

            var viewModel = BuildModel();

            viewModel.LoadIniSettingsCommand.Execute(null);
            Received
                .InOrder(() =>
                {
                    _settingsManager.ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
                    _settingsManager.LoadAllSettings();
                    _settingsManager.SaveCurrentSettings();
                });
        }

        [Test]
        public void LoadIniSettingsIsFalse_LoadSettings_NothingHappens()
        {
            var settings = new PdfCreatorSettings(new XmlStorage());

            settings.ApplicationSettings.PrimaryPrinter = "primaryPrinter";
            _simpleSettingsProvider.Settings.Returns(settings);
            _iniSettingsAssitant.LoadIniSettings().Returns(false);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.Yes);

            var viewModel = BuildModel();

            var wasSettingsLoadedCalled = false;
            viewModel.SettingsLoaded += (sender, args) => wasSettingsLoadedCalled = true;

            viewModel.LoadIniSettingsCommand.Execute(null);

            Assert.IsFalse(wasSettingsLoadedCalled);
        }

        [Test]
        public void SaveInitSettingsExecute_InitSettingAssistantGetsCalled()
        {
            var viewModel = BuildModel();

            viewModel.SaveIniSettingsCommand.Execute(null);
            Received.InOrder(() => { _iniSettingsAssitant.SaveIniSettings(_currentSettingsProvider.Settings.ApplicationSettings); });
        }
    }
}
