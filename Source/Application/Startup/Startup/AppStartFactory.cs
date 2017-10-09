using NLog;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.Core.Startup
{
    public class AppStartFactory
    {
        private readonly IAppStartResolver _appStartResolver;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        public AppStartFactory(IAppStartResolver appStartResolver)
        {
            _appStartResolver = appStartResolver;
        }

        public IAppStart CreateApplicationStart(string[] commandLineArgs)
        {
            LogCommandLineParameters(commandLineArgs);

            if (IsDragAndDropStart(commandLineArgs))
            {
                _logger.Debug("Launched Drag & Drop");
                var dragAndDropStart = _appStartResolver.ResolveAppStart<DragAndDropStart>();
                dragAndDropStart.DroppedFiles = commandLineArgs.ToList();
                return dragAndDropStart;
            }

            var commandLineParser = new CommandLineParser(commandLineArgs);

            if (commandLineParser.HasArgument("InitializeDefaultSettings"))
            {
                var defaultSettingsStart = _appStartResolver.ResolveAppStart<InitializeDefaultSettingsStart>();
                defaultSettingsStart.SettingsFile = commandLineParser.GetArgument("InitializeDefaultSettings");
                return defaultSettingsStart;
            }

            if (commandLineParser.HasArgument("StoreLicenseForAllUsers"))
            {
                var storeLicenseForAllUsers = _appStartResolver.ResolveAppStart<StoreLicenseForAllUsersStart>();

                if (commandLineParser.HasArgument("LicenseServerCode"))
                    storeLicenseForAllUsers.LicenseServerCode = commandLineParser.GetArgument("LicenseServerCode");

                if (commandLineParser.HasArgument("LicenseKey"))
                    storeLicenseForAllUsers.LicenseKey = commandLineParser.GetArgument("LicenseKey");

                return storeLicenseForAllUsers;
            }

            if (commandLineParser.HasArgument("PrintFile"))
            {
                var printFile = FindPrintFile(commandLineParser);
                var printerName = FindPrinterName(commandLineParser);

                var printFileStart = _appStartResolver.ResolveAppStart<PrintFileStart>();
                printFileStart.PrintFile = printFile;
                printFileStart.PrinterName = printerName;

                return printFileStart;
            }

            if (ShouldCallInitialize(commandLineParser))
            {
                return _appStartResolver.ResolveAppStart<InitializeSettingsStart>();
            }

            var appStart = DetermineAppStart(commandLineParser, _appStartResolver);

            if (commandLineParser.HasArgument("ManagePrintJobs"))
            {
                appStart.StartManagePrintJobs = true;
            }

            return appStart;
        }

        private void LogCommandLineParameters(string[] commandLineArguments)
        {
            if (commandLineArguments.Length > 0)
            {
                _logger.Info("Command Line parameters: \r\n" + string.Join(" ", commandLineArguments));
            }
        }

        private bool IsDragAndDropStart(string[] args)
        {
            if (args == null)
                return false;

            if (!args.Any())
                return false;

            if (args.Any(x => x.StartsWith("/") || x.StartsWith("-")))
                return false;

            if (!args.All(x => _pathSafe.IsPathRooted(x) && _pathSafe.HasExtension(x)))
                return false;

            return true;
        }

        private bool ShouldCallInitialize(CommandLineParser commandLineParser)
        {
            if (!commandLineParser.HasArgument("InitializeSettings"))
                return false;

            var excludingArguments = new[] { "ManagePrintJobs", "InfoDataFile", "PsFile", "PdfFile" };

            return excludingArguments.All(argument => !commandLineParser.HasArgument(argument));
        }

        private MaybePipedStart DetermineAppStart(CommandLineParser commandLineParser, IAppStartResolver appStartResolver)
        {
            // let's see if we have a new JobInfo passed as command line argument
            var newJob = FindJobInfoFile(commandLineParser);
            if (newJob != null)
            {
                var newPrintJobStart = appStartResolver.ResolveAppStart<NewPrintJobStart>();
                newPrintJobStart.NewJobInfoFile = newJob;
                return newPrintJobStart;
            }

            // or a PSFile?
            newJob = FindPSFile(commandLineParser);
            if (newJob != null)
            {
                var printerName = FindPrinterName(commandLineParser);

                var newPsJobStart = appStartResolver.ResolveAppStart<NewPsJobStart>();
                newPsJobStart.NewDirectConversionFile = newJob;
                newPsJobStart.PrinterName = printerName;

                return newPsJobStart;
            }

            // or a PdfFile?
            newJob = FindPdfFile(commandLineParser);
            if (newJob != null)
            {
                var printerName = FindPrinterName(commandLineParser);
                var newPdfJobStart = appStartResolver.ResolveAppStart<NewPdfJobStart>();
                newPdfJobStart.NewDirectConversionFile = newJob;
                newPdfJobStart.PrinterName = printerName;

                return newPdfJobStart;
            }

            // ...nope!? We have a MainWindowStart
            return appStartResolver.ResolveAppStart<MainWindowStart>();
        }

        private string FindPrintFile(CommandLineParser commandLineParser)
        {
            return commandLineParser.GetArgument("PrintFile");
        }

        private string FindJobInfoFile(CommandLineParser commandLineParser)
        {
            string infFile = null;

            if (!commandLineParser.HasArgument("InfoDataFile"))
                return null;

            infFile = commandLineParser.GetArgument("InfoDataFile");

            return infFile;
        }

        private string FindPSFile(CommandLineParser commandlineParser)
        {
            string psFile = null;

            if (!commandlineParser.HasArgument("PsFile"))
                return null;

            psFile = commandlineParser.GetArgument("PsFile");

            return psFile;
        }

        private string FindPdfFile(CommandLineParser commandlineParser)
        {
            string pdfFile = null;

            if (!commandlineParser.HasArgument("PdfFile"))
                return null;

            pdfFile = commandlineParser.GetArgument("PdfFile");

            return pdfFile;
        }

        private string FindPrinterName(CommandLineParser commandLineParser)
        {
            if (commandLineParser.HasArgument("PrinterName"))
                return commandLineParser.GetArgument("PrinterName");

            return "";
        }
    }
}
