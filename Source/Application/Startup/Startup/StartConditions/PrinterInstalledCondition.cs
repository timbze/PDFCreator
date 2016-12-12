using System.Collections.Generic;
using System.Linq;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class PrinterInstalledCondition : IStartupCondition
    {
        private readonly IRepairPrinterAssistant _repairPrinterAssistant;
        private readonly ISettingsLoader _settingsLoader;
        private readonly ITranslator _translator;

        public PrinterInstalledCondition(IRepairPrinterAssistant repairPrinterAssistant, ISettingsLoader settingsLoader, ITranslator translator)
        {
            _repairPrinterAssistant = repairPrinterAssistant;
            _settingsLoader = settingsLoader;
            _translator = translator;
        }

        public StartupConditionResult Check()
        {
            if (!_repairPrinterAssistant.IsRepairRequired())
                return StartupConditionResult.BuildSuccess();

            var printers = LoadPrinterNames();
            if (!_repairPrinterAssistant.TryRepairPrinter(printers))
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.PrintersBroken, _translator.GetTranslation("Application", "RepairPrinterFailed"));
            return StartupConditionResult.BuildSuccess();
        }

        private IEnumerable<string> LoadPrinterNames()
        {
            var settings = _settingsLoader.LoadPdfCreatorSettings();
            var printerMappings = settings.ApplicationSettings.PrinterMappings;
            return printerMappings.Select(mapping => mapping.PrinterName);
        }
    }
}