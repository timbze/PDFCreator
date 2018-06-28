using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction
{
    public class JobQuickActionOpenWithDefaultCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DefaultViewerAction _action;
        private readonly IFileAssoc _fileAssoc;
        private readonly ICommandLocator _commandLocator;

        public JobQuickActionOpenWithDefaultCommand(ITranslationUpdater translationUpdater, DefaultViewerAction action, IFileAssoc fileAssoc, ICommandLocator commandLocator) : base(translationUpdater)
        {
            _action = action;
            _fileAssoc = fileAssoc;
            _commandLocator = commandLocator;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object payload)
        {
            var job = (Job)payload;
            var isPdfFile = job.Profile.OutputFormat == OutputFormat.Pdf ||
                            job.Profile.OutputFormat == OutputFormat.PdfA1B ||
                            job.Profile.OutputFormat == OutputFormat.PdfA2B ||
                            job.Profile.OutputFormat == OutputFormat.PdfX;

            if (!isPdfFile)
            {
                _action.OpenOutputFile(job.OutputFiles.First());
            }
            else
            {
                if (_fileAssoc.HasOpen(".pdf"))
                {
                    _action.OpenOutputFile(job.OutputFiles.First());
                }
                else
                {
                    _commandLocator.GetCommand<JobQuickActionOpenWithPdfArchitectCommand>().Execute(payload);
                }
            }
        }
    }
}
