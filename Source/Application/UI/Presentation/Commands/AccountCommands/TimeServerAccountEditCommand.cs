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
    public class TimeServerAccountEditCommand : TranslatableCommandBase<TimeServerTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private ObservableCollection<TimeServerAccount> TimeServerAccounts => _currentSettingsProvider.Settings?.ApplicationSettings?.Accounts?.TimeServerAccounts ?? new ObservableCollection<TimeServerAccount>();
        private TimeServerAccount _currentAccount;

        public TimeServerAccountEditCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _currentSettingsProvider = currentSettingsProvider;

            TimeServerAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return TimeServerAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as TimeServerAccount;
            if (_currentAccount == null)
                return;
            if (!TimeServerAccounts.Contains(_currentAccount))
                return;

            var interaction = new TimeServerAccountInteraction(_currentAccount.Copy(), Translation.EditTimeServerAccount);
            _interactionRequest.Raise(interaction, UpdateTimeServerAccountsCallback);
        }

        private void UpdateTimeServerAccountsCallback(TimeServerAccountInteraction interaction)
        {
            if (interaction.Success)
            {
                interaction.TimeServerAccount.CopyTo(_currentAccount);
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }
            else
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
            }
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
