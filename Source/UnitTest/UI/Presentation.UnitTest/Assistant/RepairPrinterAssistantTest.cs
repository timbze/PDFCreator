using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace Presentation.UnitTest.Assistant
{
    [TestFixture]
    public class RepairPrinterAssistantTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IPrinterHelper _printerHelper;
        private IShellExecuteHelper _shellExecuteHelper;
        private IFile _file;
        private string _assemblyFolder;
        private IAssemblyHelper _assemblyHelper;
        private string _pdfcreatorPath;
        private string _printerHelperPath;
        private IPDFCreatorNameProvider _nameProvider;
        private readonly ApplicationTranslation _applicationTranslation = new ApplicationTranslation();

        [SetUp]
        public void Setup()
        {
            var exeName = "PDFCreator.exe";
            _assemblyFolder = @"X:\Programs\My Folder";
            _pdfcreatorPath = Path.Combine(_assemblyFolder, exeName);
            _printerHelperPath = Path.Combine(_assemblyFolder, "PrinterHelper.exe");

            _assemblyHelper = Substitute.For<IAssemblyHelper>();
            _assemblyHelper.GetAssemblyDirectory().Returns(_assemblyFolder);

            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _printerHelper = Substitute.For<IPrinterHelper>();
            _shellExecuteHelper = Substitute.For<IShellExecuteHelper>();
            _file = Substitute.For<IFile>();
            _nameProvider = Substitute.For<IPDFCreatorNameProvider>();
            _nameProvider.GetExeName().Returns(exeName);
            _nameProvider.GetExePath().Returns(_pdfcreatorPath);
            _nameProvider.GetPortApplicationPath().Returns(_pdfcreatorPath);
        }

        private RepairPrinterAssistant BuildRepairPrinterAssistant()
        {
            return new RepairPrinterAssistant(_interactionInvoker, _printerHelper, _shellExecuteHelper, _file, _assemblyHelper, _nameProvider, new TranslationFactory());
        }

        private void HandleMessageInteraction(MessageOptions optionsType, string message, Action<MessageInteraction> action)
        {
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var interaction = x.Arg<MessageInteraction>();
                    if ((interaction.Buttons == optionsType) && (interaction.Text == message))
                        action(interaction);
                });
        }

        [Test]
        public void IsRepairRequired_WhenPrintersExist_ReturnsFalse()
        {
            _printerHelper.GetPDFCreatorPrinters().Returns(new[] { "PDFCreator" });
            var repairPrinter = BuildRepairPrinterAssistant();

            Assert.IsFalse(repairPrinter.IsRepairRequired());
        }

        [Test]
        public void IsRepairRequired_WhenNoPrintersExist_ReturnsTrue()
        {
            _printerHelper.GetPDFCreatorPrinters().Returns(new string[] { });
            var repairPrinter = BuildRepairPrinterAssistant();

            Assert.IsTrue(repairPrinter.IsRepairRequired());
        }

        [Test]
        public void TryRepairPrinter_UserDeclines_NoRepairIsAttempted()
        {
            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.No);
            //HandleMessageInteraction(MessageOptions.OK, @"Application\RepairPrinterAskUser", interaction => interaction.Response = MessageResponse.OK);

            var repairPrinter = BuildRepairPrinterAssistant();

            var result = repairPrinter.TryRepairPrinter(new[] { "PDFCreator" });

            Assert.IsFalse(result);
            Assert.IsFalse(_shellExecuteHelper.ReceivedCalls().Any(), "ShellExecuteHelper should not have received any calls");
        }

        [Test]
        public void TryRepairPrinter_UserAgrees_RepairsPrinter()
        {
            var isRepaired = false;

            _printerHelper.GetPDFCreatorPrinters().Returns(x => isRepaired ? new[] { "PDFCreator" } : new string[] { });
            _shellExecuteHelper
                .When(x => x.RunAsAdmin(Arg.Any<string>(), Arg.Any<string>()))
                .Do(x =>
                {
                    isRepaired = true;
                });

            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.Yes);
            _file.Exists(_printerHelperPath).Returns(true);

            var repairPrinter = BuildRepairPrinterAssistant();

            var result = repairPrinter.TryRepairPrinter(new[] { "PDFCreator" });

            _shellExecuteHelper.Received()
                .RunAsAdmin(Arg.Any<string>(), "/RepairPrinter \"PDFCreator\" /PortApplication \"" + _pdfcreatorPath + "\"");
            Assert.IsTrue(result);
        }

        [Test]
        public void TryRepairPrinter_WithNoPrinterNames_RestoresPDFCreatorPrinter()
        {
            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.Yes);
            _file.Exists(_printerHelperPath).Returns(true);

            var repairPrinter = BuildRepairPrinterAssistant();

            repairPrinter.TryRepairPrinter(new string[] { });

            _shellExecuteHelper.Received()
                .RunAsAdmin(Arg.Any<string>(), "/RepairPrinter \"PDFCreator\" /PortApplication \"" + _pdfcreatorPath + "\"");
        }

        [Test]
        public void TryRepairPrinter_WithNoPrinterNamesAndDefaultPrinterNameSet_RestoresDefaultPrinter()
        {
            var expectedPrinterName = "My Printer";

            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.Yes);
            _file.Exists(_printerHelperPath).Returns(true);

            var repairPrinter = BuildRepairPrinterAssistant();
            repairPrinter.DefaultPrinterName = expectedPrinterName;

            repairPrinter.TryRepairPrinter(new string[] { });

            _shellExecuteHelper.Received()
                .RunAsAdmin(Arg.Any<string>(), $"/RepairPrinter \"{expectedPrinterName}\" /PortApplication \"{_pdfcreatorPath}\"");
        }

        [Test]
        public void TryRepairPrinter_PrinterHelperDoesNotExist_ShowsErrorAndFails()
        {
            var errorWasShown = false;

            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.Yes);
            HandleMessageInteraction(MessageOptions.OK, _applicationTranslation.GetSetupFileMissingMessage("PrinterHelper.exe"), interaction => errorWasShown = true);
            _file.Exists(_printerHelperPath).Returns(false);

            var repairPrinter = BuildRepairPrinterAssistant();

            var result = repairPrinter.TryRepairPrinter(new[] { "PDFCreator" });

            _shellExecuteHelper.DidNotReceiveWithAnyArgs().RunAsAdmin("", "");
            Assert.IsTrue(errorWasShown);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryRepairPrinter_RepairFails_ShowsErrorMessage()
        {
            HandleMessageInteraction(MessageOptions.YesNo, _applicationTranslation.RepairPrinterAskUserUac, interaction => interaction.Response = MessageResponse.Yes);
            _file.Exists(_printerHelperPath).Returns(true);

            var repairPrinter = BuildRepairPrinterAssistant();

            var result = repairPrinter.TryRepairPrinter(new[] { "PDFCreator" });

            _shellExecuteHelper.Received()
                .RunAsAdmin(Arg.Any<string>(), "/RepairPrinter \"PDFCreator\" /PortApplication \"" + _pdfcreatorPath + "\"");
            Assert.IsFalse(result);
        }
    }
}
