using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Printing.Printer
{
    public interface ISystemPrinterProvider
    {
        IEnumerable<string> GetInstalledPrinters();
    }

    public class SystemPrinterProvider : ISystemPrinterProvider
    {
        public IEnumerable<string> GetInstalledPrinters()
        {
            var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            printers.Sort();
            return printers;
        }
    }
}