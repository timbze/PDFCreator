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
    public class FtpAccountEditCommand : TranslatableCommandBase<FtpActionTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ObservableCollection<FtpAccount> _ftpAccounts;
        private FtpAccount _currentAccount;

        public FtpAccountEditCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;

            _ftpAccounts = currentSettingsProvider.Settings?.ApplicationSettings?.Accounts?.FtpAccounts ?? new ObservableCollection<FtpAccount>();
            _ftpAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _ftpAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as FtpAccount;
            if (_currentAccount == null)
                return;
            if (!_ftpAccounts.Contains(_currentAccount))
                return;

            var interaction = new FtpAccountInteraction(_currentAccount.Copy(), Translation.EditFtpAccount);
            _interactionRequest.Raise(interaction, UpdateFtpAccountsCallback);
        }

        private void UpdateFtpAccountsCallback(FtpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interaction.FtpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
