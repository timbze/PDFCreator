using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SmtpAccountAddCommand : TranslatableCommandBase<AccountsTranslation>, IWaitableCommand
    {
        private readonly ObservableCollection<SmtpAccount> _smtpAccounts;
        private readonly IInteractionRequest _interactionRequest;

        public SmtpAccountAddCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _smtpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.SmtpAccounts;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var newAccount = new SmtpAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new SmtpAccountInteraction(newAccount, Translation.AddSmtpAccount);
            _interactionRequest.Raise(interaction, AddSmtpAccountCallback);
        }

        private void AddSmtpAccountCallback(SmtpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            _smtpAccounts.Add(interaction.SmtpAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
