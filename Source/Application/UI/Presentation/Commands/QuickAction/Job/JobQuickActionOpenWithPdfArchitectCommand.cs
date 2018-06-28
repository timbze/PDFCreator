using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction
{
    public class JobQuickActionOpenWithPdfArchitectCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        private readonly DefaultViewerAction _action;
        private readonly IPdfArchitectCheck _architectCheck;
        private readonly IRecommendArchitect _recommendArchitect;

        public JobQuickActionOpenWithPdfArchitectCommand(ITranslationUpdater translationUpdater, DefaultViewerAction action, IPdfArchitectCheck architectCheck, IRecommendArchitect recommendArchitect) : base(translationUpdater)
        {
            _action = action;
            _architectCheck = architectCheck;
            _recommendArchitect = recommendArchitect;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object job)
        {
            if (_architectCheck.IsInstalled())
            {
                Job o = (Job)job;
                _action.OpenWithArchitect(o.OutputFiles);
            }
            else
            {
                _recommendArchitect.Show();
            }
        }
    }
}
