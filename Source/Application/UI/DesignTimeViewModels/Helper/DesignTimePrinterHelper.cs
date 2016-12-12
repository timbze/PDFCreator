using System.Collections.Generic;
using pdfforge.PDFCreator.Core.Printing.Printer;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimePrinterHelper : IPrinterHelper
    {
        public IList<string> GetPDFCreatorPrinters()
        {
            return new[] {"PDFCreator"};
        }

        public IList<string> GetPrinters(string portName)
        {
            return new[] {"PDFCreator"};
        }

        public string GetApplicablePDFCreatorPrinter(string requestedPrinter)
        {
            return requestedPrinter;
        }

        public string GetApplicablePDFCreatorPrinter(string requestedPrinter, string defaultPrinter)
        {
            return requestedPrinter;
        }

        public void PrintWindowsTestPage(string printerName)
        {
        }

        public string GetDefaultPrinter()
        {
            return "PDFCreator";
        }

        public bool SetDefaultPrinter(string printerName)
        {
            return true;
        }

        public bool IsValidPrinterName(string printerName)
        {
            return false;
        }
    }
}