using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class PrinterInstalledCondition : IStartupCondition
    {
        private readonly IRepairPrinterAssistant _repairPrinterAssistant;
        private readonly ISettingsLoader _settingsLoader;
        private readonly ApplicationTranslation _translation;

        public PrinterInstalledCondition(IRepairPrinterAssistant repairPrinterAssistant, ISettingsLoader settingsLoader, ApplicationTranslation translation)
        {
            _repairPrinterAssistant = repairPrinterAssistant;
            _settingsLoader = settingsLoader;
            _translation = translation;
        }

        public StartupConditionResult Check()
        {
            if (!_repairPrinterAssistant.IsRepairRequired())
                return StartupConditionResult.BuildSuccess();

            var printers = LoadPrinterNames();
            if (!_repairPrinterAssistant.TryRepairPrinter(printers))
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.PrintersBroken, _translation.RepairPrinterFailed);
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