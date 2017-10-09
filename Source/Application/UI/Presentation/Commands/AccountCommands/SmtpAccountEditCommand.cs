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
    public class SmtpAccountEditCommand : TranslatableCommandBase<SmtpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ObservableCollection<SmtpAccount> _smtpAccounts;
        private SmtpAccount _currentAccount;

        public SmtpAccountEditCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;

            _smtpAccounts = currentSettingsProvider.Settings?.ApplicationSettings?.Accounts?.SmtpAccounts ?? new ObservableCollection<SmtpAccount>();
            _smtpAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _smtpAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as SmtpAccount;
            if (_currentAccount == null)
                return;
            if (!_smtpAccounts.Contains(_currentAccount))
                return;

            var interaction = new SmtpAccountInteraction(_currentAccount.Copy(), Translation.EditSmtpAccount);
            _interactionRequest.Raise(interaction, UpdateSmtpAccountsCallback);
        }

        private void UpdateSmtpAccountsCallback(SmtpAccountInteraction interactionBase)
        {
            if (!interactionBase.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interactionBase.SmtpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
