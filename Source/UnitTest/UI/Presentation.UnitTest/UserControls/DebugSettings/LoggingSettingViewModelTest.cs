using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using SystemInterface.IO;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class LoggingSettingViewModelTest
    {
        private IInteractionInvoker _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IGpoSettings _gpoSettings;
        private IFile _fileWrap;
        private IProcessStarter _processStarter;
        private PdfCreatorSettings _ApplicationSettings;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();
            _settingsManager = Substitute.For<ISettingsManager>();
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _translationUpdater
                .When(x => x.RegisterAndSetTranslation(Arg.Any<ITranslatableViewModel<DebugSettingsTranslation>>()))
                .Do(x =>
                {
                    var viewModel = x.Arg<ITranslatableViewModel<DebugSettingsTranslation>>();
                    viewModel.Translation = new TranslationFactory().CreateTranslation<DebugSettingsTranslation>();
                });

            IStorage storage = Substitute.For<IStorage>();
            _ApplicationSettings = Substitute.For<PdfCreatorSettings>(storage);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(_ApplicationSettings);

            _gpoSettings = Substitute.For<IGpoSettings>();
            _fileWrap = Substitute.For<IFile>();
            _processStarter = Substitute.For<IProcessStarter>();
        }

        private LoggingSettingViewModel BuildViewModel()
        {
            return new LoggingSettingViewModel(_invoker, _fileWrap, _processStarter, _settingsManager, _translationUpdater, _currentSettingsProvider, _gpoSettings);
        }

        [Test]
        public void Check_Properties()
        {
            var viewModel = BuildViewModel();

            Assert.NotNull(viewModel.ShowLogFileCommand);
            Assert.NotNull(viewModel.ClearLogFileCommand);
            Assert.NotNull(viewModel.Translation);

            var loggingLevels = Enum.GetValues(typeof(LoggingLevel)) as LoggingLevel[];

            Assert.AreEqual(loggingLevels, viewModel.LoggingValues);
        }

        [Test]
        public void LogFileExists_ClearLogFileExecute_WriteToLogFile()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(true);
            var viewModel = BuildViewModel();

            viewModel.ClearLogFileCommand.Execute(null);

            Received.InOrder(() =>
            {
                _fileWrap.Exists(Arg.Any<string>());
                _fileWrap.WriteAllText(Arg.Any<string>(), Arg.Any<string>());
            });
        }

        [Test]
        public void LogFileNotExist_ClearLogFileExecute_DontWriteLogFile()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(false);
            var viewModel = BuildViewModel();

            viewModel.ClearLogFileCommand.Execute(null);

            Received.InOrder(() => _fileWrap.Exists(Arg.Any<string>()));
        }

        [Test]
        public void LoggingFileExists_ShowLogFileExecute_ProcessStarterGetsCalled()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(true);
            var viewModel = BuildViewModel();

            viewModel.ShowLogFileCommand.Execute(null);

            Received.InOrder(() =>
            {
                _fileWrap.Exists(Arg.Any<string>());
                _processStarter.Start(Arg.Any<string>());
            });
        }

        [Test]
        public void LoggingFileExistsNot_ShowLogFileExecute_UserNoLogFileInteractionIsCalled()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(false);
            var viewModel = BuildViewModel();

            _invoker
                .When(x => _invoker.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var message = x.Arg<MessageInteraction>();
                    Assert.AreEqual(viewModel.Translation.NoLogFile, message.Title);
                    Assert.AreEqual(viewModel.Translation.NoLogFileAvailable, message.Text);
                    Assert.AreEqual(message.Buttons, MessageOptions.OK);
                    Assert.AreEqual(message.Icon, MessageIcon.Warning);
                });

            viewModel.ShowLogFileCommand.Execute(null);
        }
    }
}
