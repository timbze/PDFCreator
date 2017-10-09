using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class FtpAccountAddCommand : TranslatableCommandBase<FtpActionTranslation>, IWaitableCommand
    {
        private readonly ObservableCollection<FtpAccount> _ftpAccounts;
        private readonly IInteractionRequest _interactionRequest;

        public FtpAccountAddCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _ftpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.FtpAccounts;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var newAccount = new FtpAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new FtpAccountInteraction(newAccount, Translation.AddFtpAccount);
            _interactionRequest.Raise(interaction, AddFtpAccountCallback);
        }

        private void AddFtpAccountCallback(FtpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            _ftpAccounts.Add(interaction.FtpAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
