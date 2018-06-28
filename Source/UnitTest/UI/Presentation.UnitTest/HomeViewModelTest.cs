using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class HomeViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _fileConversionHandler = Substitute.For<IFileConversionHandler>();
            var printerHelper = Substitute.For<IPrinterHelper>();
            printerHelper.GetApplicablePDFCreatorPrinter(Arg.Any<string>()).Returns("Some Default Printer");
            var settingsProvider = Substitute.For<ISettingsProvider>();
            var jobHistoryManager = Substitute.For<IJobHistoryManager>();
            var commandLocator = Substitute.For<ICommandLocator>();

            HomeViewModel = new HomeViewModel(_interactionInvoker, _fileConversionHandler, new DesignTimeTranslationUpdater(), printerHelper,
                settingsProvider, jobHistoryManager, new InvokeImmediatelyDispatcher(), commandLocator);
        }

        private IInteractionInvoker _interactionInvoker;
        private IFileConversionHandler _fileConversionHandler;

        public HomeViewModel HomeViewModel { get; set; }

        [Test]
        public void Check_Properties()
        {
            Assert.NotNull(HomeViewModel.Translation);
        }

        [Test]
        public void ConvertFileCommand_WhenFileWasSelected_ConvertsFile()
        {
            var selectedFile = @"Some\Path\To\File.txt";

            _interactionInvoker.When(i => i.Invoke(Arg.Any<OpenFileInteraction>())).Do(ci =>
            {
                var interaction = ci.Arg<OpenFileInteraction>();
                interaction.Success = true;
                interaction.FileName = selectedFile;
            });

            HomeViewModel.ConvertFileCommand.Execute(null);

            _interactionInvoker.Received(1).Invoke(Arg.Any<OpenFileInteraction>());
            _fileConversionHandler.Received(1).HandleFileList(Arg.Is<List<string>>(l => l.Single().Equals(selectedFile)));
        }

        [Test]
        public void ConvertFileCommand_WhenInteractionWasNotSuccessful_DoesNothing()
        {
            HomeViewModel.ConvertFileCommand.Execute(null);

            _interactionInvoker.Received(1).Invoke(Arg.Any<OpenFileInteraction>());
            _fileConversionHandler.DidNotReceiveWithAnyArgs().HandleFileList(Arg.Any<List<string>>());
        }
    }
}
