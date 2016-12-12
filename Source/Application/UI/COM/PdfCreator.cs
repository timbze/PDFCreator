using System.Runtime.InteropServices;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Printing.Printer;

namespace pdfforge.PDFCreator.UI.COM
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A1F6647E-8C19-4A3E-89DF-7FDFAD2A0C30")]
    public interface IPDFCreator
    {
        Printers GetPDFCreatorPrinters { get; }
        bool IsInstanceRunning { get; }
        void PrintFile(string path);
        void AddFileToQueue(string path);
        void PrintFileSwitchingPrinters(string path, bool allowDefaultPrinterSwitch);
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("69189C58-70C4-4DF2-B94D-5D786E9AD513")]
    [ProgId("PDFCreator.PDFCreatorObj")]
    public class PdfCreatorObj : IPDFCreator
    {
        private readonly PdfCreatorAdapter _pdfCreatorAdapter;
        private readonly IPrinterHelper _printerHelper;

        public PdfCreatorObj()
        {
            var builder = new ComDependencyBuilder();
            var dependencies = builder.ComDependencies;
            _pdfCreatorAdapter = dependencies.PdfCreatorAdapter;
            _printerHelper = _pdfCreatorAdapter.PrinterHelper;
        }

        /// <summary>
        ///     Returns a new PrinterDevices object
        /// </summary>
        public Printers GetPDFCreatorPrinters
        {
            get { return new Printers(_printerHelper); }
        }

        /// <summary>
        ///     Checks if PDFCreator is running
        /// </summary>
        public bool IsInstanceRunning => _pdfCreatorAdapter.IsInstanceRunning;

        /// <summary>
        ///     Prints a file to the PDFCreator printer without switching the default printer.
        /// </summary>
        /// <param name="path">Path of the file to be printed</param>
        public void PrintFile(string path)
        {
            _pdfCreatorAdapter.PrintFile(path);
        }

        /// <summary>
        ///     Prints a file to the PDFCreator printer, where the user of the COM Interface can decide,
        ///     if the default printer should be temporally changed or not. In case of not, the file
        ///     will not be printed.
        /// </summary>
        /// <param name="path">The path to the file that should be printed</param>
        /// <param name="allowDefaultPrinterSwitch">
        ///     If true, the default printer will be temporally changed,
        ///     otherwise not and the file will not be printed.
        /// </param>
        public void PrintFileSwitchingPrinters(string path, bool allowDefaultPrinterSwitch)
        {
            _pdfCreatorAdapter.PrintFileSwitchingPrinters(path, allowDefaultPrinterSwitch);
        }

        /// <summary>
        ///     Inserts a .ps or .pdf file directly into the queue.
        /// </summary>
        /// <param name="path">The path to the .ps or .pdf file</param>
        public void AddFileToQueue(string path)
        {
            _pdfCreatorAdapter.AddFileToQueue(path);
        }
    }
}
