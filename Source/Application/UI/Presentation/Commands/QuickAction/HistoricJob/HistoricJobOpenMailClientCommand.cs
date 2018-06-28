using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction
{
    public class HistoricJobOpenMailClientCommand : TranslatableCommandBase<FtpActionTranslation>
    {
        private readonly EMailClientAction _action;

        public HistoricJobOpenMailClientCommand(ITranslationUpdater translationUpdater, EMailClientAction action) : base(translationUpdater)
        {
            _action = action;
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

            _action.Process("", "", false, false, "", "", "", "", true, files);
        }
    }
}
