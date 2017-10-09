using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Core.Printing.Port;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Printing
{
    public class FolderProvider : ITempFolderProvider, ISpoolerProvider
    {
        private const string PrinterPortName = "pdfcmon";
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IPrinterPortReader _printerPortReader;

        public FolderProvider(IPrinterPortReader printerPortReader, IPath path)
        {
            _printerPortReader = printerPortReader;
            _path = path;

            var tempFolderBase = GetTempFolderBase();

            TempFolder = _path.Combine(tempFolderBase, "Temp");
            _logger.Debug("Temp folder is '{0}'", TempFolder);

            SpoolFolder = _path.Combine(tempFolderBase, "Spool");
            _logger.Debug("Spool folder is '{0}'", SpoolFolder);
        }

        public string SpoolFolder { get; }

        public string TempFolder { get; }

        private string GetTempFolderBase()
        {
            var tempFolderName = ReadTempFolderName();
            return _path.Combine(_path.GetTempPath(), tempFolderName);
        }

        private string ReadTempFolderName()
        {
            var printerPort = _printerPortReader.ReadPrinterPort(PrinterPortName);

            if (printerPort == null)
                return "PDFCreator";

            return printerPort.TempFolderName;
        }
    }
}
