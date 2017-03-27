using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.Assistants
{
    [TestFixture]
    public class IniSettingsAssistantTest
    {
        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();
            _settingsHelper = Substitute.For<ISettingsProvider>();
            _settingsHelper.Settings.Returns(new PdfCreatorSettings(Substitute.For<IStorage>()));

            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_settingsHelper);

            _dataStorageFactory = Substitute.For<IDataStorageFactory>();
            _storage = Substitute.For<IStorage>();
            _dataStorageFactory.BuildIniStorage().Returns(_storage);

            _iniSettingsLoader = Substitute.For<IIniSettingsLoader>();
            _iniSettingsAssistant = new IniSettingsAssistant(_invoker, new ApplicationSettingsWindowTranslation(), _settingsManager, _dataStorageFactory, _iniSettingsLoader);
        }

        private IInteractionInvoker _invoker;
        private IniSettingsAssistant _iniSettingsAssistant;
        private IStorage _storage;
        private IDataStorageFactory _dataStorageFactory;
        private ISettingsProvider _settingsHelper;
        private ISettingsManager _settingsManager;
        private IIniSettingsLoader _iniSettingsLoader;

        private void SetSuccessfulOpenFileInteraction(string filename)
        {
            _invoker.When(x => x.Invoke(Arg.Any<OpenFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<OpenFileInteraction>();
                interaction.Success = true;
                interaction.FileName = filename;
            });
        }

        [Test]
        public void LoadIniSettings_IfNoFileNameWasSelected_ReturnsWithoutLoad()
        {
            _iniSettingsAssistant.LoadIniSettings();
            _dataStorageFactory.DidNotReceive().BuildIniStorage();
        }

        [Test]
        public void LoadIniSettings_IfOverwriteIsAllowed_LoadsSelectedFile()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(true);

            _iniSettingsAssistant.LoadIniSettings();

            _iniSettingsLoader.Received().LoadIniSettings(filename);
            _settingsManager.Received().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }

        [Test]
        public void LoadIniSettings_IfOverwriteWasNotAllowed_ReturnsWithoutLoad()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _iniSettingsAssistant.LoadIniSettings();
            _dataStorageFactory.DidNotReceive().BuildIniStorage();
        }

        [Test]
        public void LoadIniSettings_WithInvalidSettings_DisplayWarningAndDoesNotApplySettings()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => { info.Arg<MessageInteraction>().Response = MessageResponse.Yes; });
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(false);

            _iniSettingsAssistant.LoadIniSettings();

            _invoker.Received(2).Invoke(Arg.Any<MessageInteraction>());

            _settingsManager.DidNotReceive().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }

        [Test]
        public void SaveIniSettings_IfNoFilenameWasSelected_ReturnsWithoutSave()
        {
            var appSettings = new ApplicationSettings();

            _iniSettingsAssistant.SaveIniSettings(appSettings);
            _dataStorageFactory.DidNotReceive().BuildIniStorage();
        }

        [Test]
        public void SaveIniSettings_SavesToSelectedFile()
        {
            const string filename = "MyFileName";
            _invoker.When(x => x.Invoke(Arg.Any<SaveFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<SaveFileInteraction>();
                interaction.Success = true;
                interaction.FileName = filename;
            });

            var appSettings = new ApplicationSettings();

            _iniSettingsAssistant.SaveIniSettings(appSettings);

            _storage.Received().WriteData(filename);
        }
    }
}