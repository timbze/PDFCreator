using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class TimeServerAccountAddCommand : TranslatableCommandBase<TimeServerTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        public TimeServerAccountAddCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _currentSettingsProvider = currentSettingsProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var newAccount = new TimeServerAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new TimeServerAccountInteraction(newAccount, Translation.AddTimeServerAccount);
            _interactionRequest.Raise(interaction, AddTimeServerAccountCallback);
        }

        private void AddTimeServerAccountCallback(TimeServerAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            var timeServerAccounts = _currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.TimeServerAccounts;
            timeServerAccounts?.Add(interaction.TimeServerAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
