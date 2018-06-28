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
    public class ADebugSettingsItemControlModelTest
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

            var pdfCreatorSettings = new PdfCreatorSettings(new IniStorage());
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

        public ADebugSettingsItemControlModel BuildModel()
        {
            return new RestoreSettingsViewModel(_invoker, _settingsManager, _translationUpdater, _currentSettingsProvider, _gpoSettings);
        }

        [Test]
        public void GetProperties_AllPropertiesAreSet()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.GpoSettings);
            Assert.NotNull(viewModel.Translation);
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _invoker
                .When(x => x.Raise(Arg.Any<MessageInteraction>(), Arg.Any<Action<MessageInteraction>>()))
                .Do(x =>
                {
                    var interaction = x.Arg<MessageInteraction>();
                    action(interaction);
                    interaction.Response = MessageResponse.Yes;

                    x.Arg<Action<MessageInteraction>>().Invoke(interaction);
                });
        }

        [Test]
        public void CreateNewSettings_SetNewSettings_SettingsAreSetAndEventIsCalled()
        {
            var settings = new PdfCreatorSettings(new XmlStorage());

            settings.ApplicationSettings.PrimaryPrinter = "primaryPrinter";
            _simpleSettingsProvider.Settings.Returns(settings);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.Yes);

            var viewModel = BuildModel();

            var wasSettingsLoadedCalled = false;

            viewModel.SettingsLoaded += (sender, args) => wasSettingsLoadedCalled = true;

            (viewModel as RestoreSettingsViewModel).RestoreDefaultSettingsCommand.Execute(null);

            Received.InOrder(() =>
            {
                _settingsManager.ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
                _settingsManager.LoadAllSettings();
                _settingsManager.SaveCurrentSettings();
            });

            Assert.IsTrue(wasSettingsLoadedCalled);
        }
    }
}
