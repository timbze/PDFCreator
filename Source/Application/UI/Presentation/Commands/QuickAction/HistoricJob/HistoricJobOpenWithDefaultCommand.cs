using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction.HistoricJob
{
    public class HistoricJobOpenWithDefaultCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DefaultViewerAction _action;
        private readonly IFileAssoc _fileAssoc;
        private readonly ICommandLocator _commandLocator;

        public HistoricJobOpenWithDefaultCommand(ITranslationUpdater translationUpdater, DefaultViewerAction action, IFileAssoc fileAssoc, ICommandLocator commandLocator) : base(translationUpdater)
        {
            _action = action;
            _fileAssoc = fileAssoc;
            _commandLocator = commandLocator;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object obj)
        {
            var historicJob = obj as Core.Services.JobHistory.HistoricJob;
            if (historicJob == null)
                return;

            var firstFilePath = historicJob.HistoricFiles.First().Path;
            var isPdfFile = firstFilePath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);

            if (!isPdfFile)
            {
                _action.OpenOutputFile(firstFilePath);
            }
            else
            {
                if (_fileAssoc.HasOpen(".pdf"))
                {
                    _action.OpenOutputFile(firstFilePath);
                }
                else
                {
                    _commandLocator.GetCommand<HistoricJobOpenWithPdfArchitectCommand>().Execute(obj);
                }
            }
        }
    }
}
