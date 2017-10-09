using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace Presentation.UnitTest.Assistant
{
    [TestFixture]
    public class PrinterActionAssitantTest
    {
        private IPrinterHelper _printerHelper;
        private IUacAssistant _uacAssistant;
        private IInteractionRequest _interactionRequest;
        public PrinterActionsAssistant PrinterActionsAssistant;

        [SetUp]
        public void Setup()
        {
            _printerHelper = Substitute.For<IPrinterHelper>();
            _uacAssistant = Substitute.For<IUacAssistant>();
            _interactionRequest = Substitute.For<IInteractionRequest>();

            PrinterActionsAssistant = new PrinterActionsAssistant(_printerHelper, _uacAssistant, new DesignTimeTranslationUpdater(), _interactionRequest);
            ;
        }

        [Test]
        public void DeltePrinter_PrintersLower2_ReturnsFalse()
        {
            var numPrinters = 1;

            var methodResult = PrinterActionsAssistant.DeletePrinter("SomePrintername", numPrinters).Result;

            Assert.IsFalse(methodResult);
        }

        [Test]
        public void DeltePrinter_PrintersHigher2_ReturnsSameResultAsUacAssistant()
        {
            var numPrinters = 3;
            var printername = "SomePrintername";
            var methodResult = PrinterActionsAssistant.DeletePrinter(printername, numPrinters).Result;

            Assert.AreEqual(_uacAssistant.DeletePrinter(printername), methodResult);
        }

        [Test]
        public void RenamePrinter_OldPrinternameIsEmpty_ReturnsNull()
        {
            var oldPrintername = string.Empty;
            var methodResult = PrinterActionsAssistant.RenamePrinter(oldPrintername).Result;

            Assert.IsNull(methodResult);
        }
    }
}
