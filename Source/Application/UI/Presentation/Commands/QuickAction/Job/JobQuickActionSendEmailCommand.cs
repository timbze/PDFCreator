using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction
{
    public class JobQuickActionSendEmailCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        private readonly EMailClientAction _action;

        public JobQuickActionSendEmailCommand(ITranslationUpdater translationUpdater, EMailClientAction action) : base(translationUpdater)
        {
            _action = action;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object p_Job)
        {
            var job = (Job)p_Job;
            _action.Process("", "", false, true, job.JobTranslations.EmailSignature, "", "", "", true, job.OutputFiles);
        }
    }
}
