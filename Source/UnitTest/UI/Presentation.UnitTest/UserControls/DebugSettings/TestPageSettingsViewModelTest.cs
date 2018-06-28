using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
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

        private TestPageSettingsViewModel SetupPdfCreatorTestPagePreperations()
        {
            var viewModel = BuildViewModel();

            return viewModel;
        }

        [Test]
        public void RequestTestPage_CreatesPage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations();

            var wasCalled = false;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => wasCalled = true);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
