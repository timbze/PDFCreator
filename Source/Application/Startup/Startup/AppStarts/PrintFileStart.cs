using System.IO;
using NLog;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class PrintFileStart : AppStartBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrintFileHelper _printFileHelper;
        private readonly ISettingsLoader _settingsLoader;

        public PrintFileStart(ICheckAllStartupConditions startupConditions, IPrintFileHelper printFileHelper, ISettingsLoader settingsLoader)
            : base(startupConditions)
        {
            _printFileHelper = printFileHelper;
            _settingsLoader = settingsLoader;
        }

        public string PrintFile { get; internal set; }
        public string PrinterName { get; internal set; }

        public override ExitCode Run()
        {
            _logger.Info("Launched printjob with PrintFile command.");

            if (string.IsNullOrEmpty(PrintFile))
            {
                _logger.Error("PrintFile Parameter has no argument");
                return ExitCode.PrintFileParameterHasNoArgument;
;
            }

            if (!File.Exists(PrintFile))
            {
                _logger.Error("The file \"{0}\" does not exist!", PrintFile);
                return ExitCode.PrintFileDoesNotExist;
            }

            _printFileHelper.PdfCreatorPrinter = GetValidPrinterName();

            if (!_printFileHelper.AddFile(PrintFile))
            {
                _logger.Warn("The file \"{0}\" is not printable!", PrintFile);
                return ExitCode.PrintFileNotPrintable;
            }

            if (!_printFileHelper.PrintAll())
            {
                _logger.Error("The file \"{0}\" could not be printed!", PrintFile);
                return ExitCode.PrintFileCouldNotBePrinted;
            }

            return ExitCode.Ok;
        }

        private string GetValidPrinterName()
        {
            if (!string.IsNullOrWhiteSpace(PrinterName))
                return PrinterName;

            var settings = _settingsLoader.LoadPdfCreatorSettings();
            return settings.ApplicationSettings.PrimaryPrinter;
        }
    }
}