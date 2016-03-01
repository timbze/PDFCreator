using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SystemInterface.IO;
using SystemWrapper.IO;
using NLog;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Shared.Assistants;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Core.Settings;

namespace pdfforge.PDFCreator.Assistants
{
    internal class RepairPrinterAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAssemblyHelper _assemblyHelper = new AssemblyHelper();
        private readonly Translator _translator = TranslationHelper.Instance.TranslatorInstance;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        public bool TryRepairPrinter(IEnumerable<string> printerNames)
        {
            Logger.Error("It looks like the printers are broken. This needs to be fixed to allow PDFCreator to work properly");

            string title = _translator.GetTranslation("Application", "RepairPrinterNoPrintersInstalled", "No printers installed");
            string message = _translator.GetFormattedTranslation("Application", "RepairPrinterAskUser", "You do not have any PDFCreator printers installed. Most likely there was a problem during the setup or the installation has been altered afterwards.\r\nDo you want to fix this by reinstalling the PDFCreator printers?\r\n\r\nNote: You might be asked twice to grant admin privileges while fixing the problem.");

            Logger.Debug("Asking to start repair..");

            if (MessageWindow.ShowTopMost(message, title, MessageWindowButtons.YesNo, MessageWindowIcon.Exclamation) == MessageWindowResponse.Yes)
            {
                var applicationPath = _assemblyHelper.GetCurrentAssemblyDirectory();
                var printerHelperPath = _pathSafe.Combine(applicationPath, "PrinterHelper.exe");

                if (!File.Exists(printerHelperPath))
                {
                    Logger.Error("PrinterHelper.exe does not exist!");
                    title = _translator.GetTranslation("Application", "Error", "Error");
                    message = _translator.GetFormattedTranslation("Application", "SetupFileMissing",
                        "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!", _pathSafe.GetFileName(printerHelperPath));

                    MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.Error);
                    return false;
                }

                var shellExecuteHelper = new ShellExecuteHelper();

                Logger.Debug("Uninstalling Printers...");
                var uninstallResult = shellExecuteHelper.RunAsAdmin(printerHelperPath, "/UninstallPrinter");
                Logger.Debug("Done: {0}", uninstallResult);

                Logger.Debug("Reinstalling Printers...");
                var pdfcreatorPath = _pathSafe.Combine(applicationPath, "PDFCreator.exe");

                string printerNameString = GetPrinterNameString(printerNames);

                var installParams = $"/InstallPrinter {printerNameString} /PortApplication \"{pdfcreatorPath}\"";
                var installResult = shellExecuteHelper.RunAsAdmin(printerHelperPath, installParams);
                Logger.Debug("Done: {0}", installResult);
            }

            Logger.Debug("Now we'll check again, if the printer is installed");
            if (IsRepairRequired())
            {
                Logger.Info("The printer could not be repaired.");
                title = _translator.GetTranslation("Application", "Error", "Error");
                message = _translator.GetFormattedTranslation("Application", "RepairPrinterFailed",
                    "PDFCreator was not able to repair your printers. Please contact your administrator or the support to assist you in with this problem.");

                MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.Exclamation);
                return false;
            }

            Logger.Info("The printer was repaired successfully");

            return true;
        }

        private string GetPrinterNameString(IEnumerable<string> printerNames)
        {
            var printers = printerNames.ToList();

            if (!printers.Any())
                printers.Add("PDFCreator");

            return string.Join(" ", printers.Select(printerName => "\"" + printerName + "\""));
        }

        public bool IsRepairRequired()
        {
            var printerHelper = new PrinterHelper();
            return !printerHelper.GetPDFCreatorPrinters().Any();
        }
    }
}
