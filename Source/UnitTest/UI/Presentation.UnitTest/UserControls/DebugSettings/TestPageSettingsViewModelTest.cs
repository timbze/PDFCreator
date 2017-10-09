using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class TestPageSettingsViewModelTest
    {
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

            var storage = Substitute.For<IStorage>();
            _applicationSettings = Substitute.For<PdfCreatorSettings>(storage);
            _settingsManager.GetSettingsProvider().Settings.Returns(_applicationSettings);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(_applicationSettings);

            _gpoSettings = Substitute.For<IGpoSettings>();
            _testPageHelper = Substitute.For<ITestPageHelper>();
            _printHelper = Substitute.For<IPrinterHelper>();
        }

        private IInteractionInvoker _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IGpoSettings _gpoSettings;
        private PdfCreatorSettings _applicationSettings;
        private ITestPageHelper _testPageHelper;
        private IPrinterHelper _printHelper;

        private TestPageSettingsViewModel BuildViewModel()
        {
            return new TestPageSettingsViewModel(_testPageHelper, _printHelper, _settingsManager, _translationUpdater, _invoker, _currentSettingsProvider, _gpoSettings);
        }

        [Test]
        public void Check_Properties()
        {
            var viewModel = BuildViewModel();

            Assert.NotNull(viewModel.PrintPdfCreatorTestpageCommand);
            Assert.NotNull(viewModel.PrintWindowsTestpageCommand);
            Assert.NotNull(viewModel.Translation);
        }

        private TestPageSettingsViewModel SetupPdfCreatorTestPagePreperations(bool differentSettings)
        {
            var storage = Substitute.For<IStorage>();

            var viewModel = BuildViewModel();

            if (differentSettings)
            {
                var settingsDifferent = Substitute.For<PdfCreatorSettings>(storage);
                settingsDifferent.ApplicationSettings.PrimaryPrinter = "OtherPrinter";
                _currentSettingsProvider.Settings.Returns(settingsDifferent);
            }

            return viewModel;
        }

        [Test]
        public void ModifiedSettingsUserCanceledInteraction_RequestTestPage_DoesntCreatePage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations(true);

            var wasSilent = true;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => wasSilent = false);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Assert.IsTrue(wasSilent);
        }

        [Test]
        public void ModifiedSettingsUserCanceledInteraction_RequestWindowsTestPage_DoesntCreateWindowsPage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations(true);

            var wasSilent = true;
            _printHelper
                .When(x => _printHelper.PrintWindowsTestPage(Arg.Any<string>()))
                .Do(x => wasSilent = false);

            viewModel.PrintWindowsTestpageCommand.Execute(null);

            Assert.IsTrue(wasSilent);
        }

        [Test]
        public void UnModifiedSettings_UserCanceledInteraction_RequestTestPage_CreatesPage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations(false);

            var wasCalled = false;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => wasCalled = true);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ModifiedSettings_UserAcceptInteraction_RequestTestPage_CreatesPage()
        {
            _invoker
                .When(invoker => invoker.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var message = x.Arg<MessageInteraction>();
                    message.Response = MessageResponse.Yes;
                });

            var viewModel = SetupPdfCreatorTestPagePreperations(true);

            var wasCalled = false;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => wasCalled = true);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Received
                .InOrder(() =>
                {
                    _currentSettingsProvider.Settings.ApplicationSettings = _currentSettingsProvider.Settings.ApplicationSettings.Copy();
                    _settingsManager.ApplyAndSaveSettings(_currentSettingsProvider.Settings);
                });

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ModifiedSettings_UserDeclinedInteraction_RequestTestPage_CreatesPage()
        {
            _invoker
                .When(invoker => invoker.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var message = x.Arg<MessageInteraction>();
                    message.Response = MessageResponse.No;
                });

            var viewModel = SetupPdfCreatorTestPagePreperations(true);

            var openPageWasCalled = false;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => openPageWasCalled = true);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Assert.IsTrue(openPageWasCalled);
        }

        [Test]
        public void UnModifiedSettings_UserDeclinedInteraction_RequestWindowsTestPage_CreatesWindowsTestPage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations(false);

            var openPageWasCalled = false;
            _printHelper
                .When(x => _printHelper.PrintWindowsTestPage(Arg.Any<string>()))
                .Do(x => openPageWasCalled = true);

            viewModel.PrintWindowsTestpageCommand.Execute(null);

            Assert.IsTrue(openPageWasCalled);
        }
    }
}
