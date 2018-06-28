using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction.HistoricJob
{
    public class HistoricJobOpenWithPdfArchitectCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        private readonly DefaultViewerAction _action;
        private readonly IPdfArchitectCheck _architectCheck;
        private readonly IRecommendArchitect _recommendArchitect;

        public HistoricJobOpenWithPdfArchitectCommand(ITranslationUpdater translationUpdater, DefaultViewerAction action, IPdfArchitectCheck architectCheck, IRecommendArchitect recommendArchitect) : base(translationUpdater)
        {
            _action = action;
            _architectCheck = architectCheck;
            _recommendArchitect = recommendArchitect;
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

            var files = new List<string>();
            foreach (var historicFile in historicJob.HistoricFiles)
                files.Add(historicFile.Path);

            if (files.Count == 0)
                return;

            if (_architectCheck.IsInstalled())
            {
                _action.OpenWithArchitect(files);
            }
            else
            {
                _recommendArchitect.Show();
            }
        }
    }
}
