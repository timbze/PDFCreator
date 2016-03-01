

using System.Drawing.Printing;

namespace pdfforge.PDFCreator.Utilities
{
    public class PrinterWrapper
    {
        private readonly PrinterSettings _printer;

        public PrinterWrapper()
        {
            _printer = new PrinterSettings();
        }

        public virtual string PrinterName
        {
            get { return _printer.PrinterName; }
            set { _printer.PrinterName = value; }
        }
        
        public virtual bool IsValid
        {
            get { return _printer.IsValid; }
        }

        public virtual bool CanDuplex
        {
            get { return _printer.CanDuplex; }
        }
    }
}
