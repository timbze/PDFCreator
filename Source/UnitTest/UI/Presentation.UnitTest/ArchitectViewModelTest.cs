using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Architect;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class ArchitectViewModelTest
    {
        private IProcessStarter _processStarter;
        private IPdfArchitectCheck _pdfArchitectCheck;

        [SetUp]
        public void Setup()
        {
            _processStarter = Substitute.For<IProcessStarter>();
            _pdfArchitectCheck = Substitute.For<IPdfArchitectCheck>();
        }

        private ArchitectViewModel BuildArchitectViewModel()
        {
            return new ArchitectViewModel(_pdfArchitectCheck, _processStarter, new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
        }

        [Test]
        public void CreateViewModel_CheckPDFArchitectIsInstalled_PDFArchitectInstalledCheckedIsCalled()
        {
            var viewModel = BuildArchitectViewModel();

            _pdfArchitectCheck.IsInstalled().Returns(true);
            Assert.IsTrue(viewModel.IsPdfArchitectInstalled);

            _pdfArchitectCheck.IsInstalled().Returns(false);
            Assert.IsFalse(viewModel.IsPdfArchitectInstalled);
        }

        [Test]
        public void CreateViewModel_CallLaunchPDFArchitectCommand_ProcessStarterIsCalledWithApplicationPath()
        {
            var viewModel = BuildArchitectViewModel();
            var appPath = "TestPath";
            _pdfArchitectCheck.GetInstallationPath().Returns(appPath);

            viewModel.LaunchPdfArchitectCommand.Execute(null);

            _processStarter.Received().Start(Arg.Is(appPath));
        }

        [Test]
        public void CreateViewModel_CallLaunchPDFArchitectCommandWithBrokenString_ProcessStarterIsNotCalled()
        {
            var viewModel = BuildArchitectViewModel();
            var appPath = "   ";
            _pdfArchitectCheck.GetInstallationPath().Returns(appPath);

            viewModel.LaunchPdfArchitectCommand.Execute(null);
            _processStarter.DidNotReceive().Start(Arg.Any<string>());
        }

        [Test]
        public void CreateViewModel_CallWebsiteCommand_ProcessStarterIsCalledWithArchitectWebsiteUrl()
        {
            var viewModel = BuildArchitectViewModel();

            viewModel.LaunchWebsiteCommand.Execute(null);
            _processStarter.Received().Start(Arg.Is(Urls.ArchitectWebsiteUrl));
        }

        [Test]
        public void CreateViewModel_CallDownloadCommand_ProcessStarterIsCalledWithArchitectDownloadUrl()
        {
            var viewModel = BuildArchitectViewModel();

            viewModel.DownloadPdfArchitectCommand.Execute(null);
            _processStarter.Received().Start(Arg.Is(Urls.ArchitectDownloadUrl));
        }

        [Test]
        public void CreateViewModel_GetTranslation_TranslationWasCreated()
        {
            var viewModel = BuildArchitectViewModel();

            ArchitectViewTranslation translation = viewModel.Translation;
            Assert.NotNull(translation);
        }
    }
}
