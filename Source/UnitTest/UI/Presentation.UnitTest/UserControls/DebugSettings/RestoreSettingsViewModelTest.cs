using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class RestoreSettingsViewModelTest
    {
        private IInteractionRequest _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IGpoSettings _gpoSettings;
        private ISettingsProvider _simpleSettingsProvider;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionRequest>();

            IStorage storage = Substitute.For<IStorage>();
            var pdfCreatorSettings = new PdfCreatorSettings(storage);

            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(pdfCreatorSettings);

            _gpoSettings = Substitute.For<IGpoSettings>();

            _simpleSettingsProvider = Substitute.For<ISettingsProvider>();
            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_simpleSettingsProvider);

            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        public RestoreSettingsViewModel BuildModel()
        {
            return new RestoreSettingsViewModel(_invoker, _settingsManager, _translationUpdater, _currentSettingsProvider, _gpoSettings);
        }

        [Test]
        public void GetProperties_AllPropertiesAreSet()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.RestoreDefaultSettingsCommand);
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _invoker
                .When(x => x.Raise(Arg.Any<MessageInteraction>()))
                .Do(x => action(x.Arg<MessageInteraction>()));
        }

        [Test]
        public void RestoreDefaultSettings_FailInteraction_DoNotChangeSettings()
        {
            IStorage storage = Substitute.For<IStorage>();
            var settings = new PdfCreatorSettings(storage) { ApplicationSettings = { PrimaryPrinter = "primaryPrinter" } };

            _simpleSettingsProvider.Settings.Returns(settings);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.No);

            var viewModel = BuildModel();

            var wasSettingsLoadedCalled = false;

            viewModel.SettingsLoaded += (sender, args) => wasSettingsLoadedCalled = true;

            viewModel.RestoreDefaultSettingsCommand.Execute(null);

            Assert.IsFalse(wasSettingsLoadedCalled);
        }
    }
}
