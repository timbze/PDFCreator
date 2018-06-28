using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenMailClientCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IEMailClientAction _action;

        public QuickActionOpenMailClientCommand(ITranslationUpdater translationUpdater, IEMailClientAction action) : base(translationUpdater)
        {
            _action = action;
        }

        public override void Execute(object obj)
        {
            List<string> files = GetPaths(obj);
            var signature = GetSignature(obj);

            _action.Process("", "", false, true, signature, "", "", "", true, files);
        }
    }
}
