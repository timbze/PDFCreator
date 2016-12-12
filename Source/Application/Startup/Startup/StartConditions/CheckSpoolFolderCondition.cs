using NLog;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class CheckSpoolFolderCondition : IStartupCondition
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRepairSpoolFolderAssistant _repairSpoolFolderAssistant;
        private readonly ITranslator _translator;
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly ISpoolFolderAccess _spoolFolderAccess;

        public CheckSpoolFolderCondition(ISpoolFolderAccess spoolFolderAccess, IRepairSpoolFolderAssistant repairSpoolFolderAssistant, ITranslator translator, ISpoolerProvider spoolerProvider)
        {
            _spoolFolderAccess = spoolFolderAccess;
            _repairSpoolFolderAssistant = repairSpoolFolderAssistant;
            _translator = translator;
            _spoolerProvider = spoolerProvider;
        }

        public StartupConditionResult Check()
        {
            if (_spoolFolderAccess.CanAccess())
                return StartupConditionResult.BuildSuccess();

            _repairSpoolFolderAssistant.TryRepairSpoolPath();

            _logger.Debug("Now we'll check again, if the spool folder is not accessible");

            if (!_spoolFolderAccess.CanAccess())
            {
                _logger.Info("The spool folder could not be repaired.");
                _repairSpoolFolderAssistant.DisplayRepairFailedMessage();
                var message = _translator.GetFormattedTranslation("Application", "SpoolFolderUnableToRepair", _spoolerProvider.SpoolFolder);
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.SpoolFolderInaccessible, message);
            }
            _logger.Info("The spool folder was repaired successfully.");
            return StartupConditionResult.BuildSuccess();
        }
    }
}