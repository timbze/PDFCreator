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
    public class HttpAccountEditCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private HttpAccount _currentAccount;

        public HttpAccountEditCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _currentSettingsProvider = currentSettingsProvider;

            if (_currentSettingsProvider.Settings != null)
            {
                var httpAccounts = _currentSettingsProvider.Settings.ApplicationSettings.Accounts.HttpAccounts;
                httpAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (_currentSettingsProvider.Settings == null)
                return false;

            var httpAccounts = _currentSettingsProvider.Settings.ApplicationSettings.Accounts.HttpAccounts;

            return httpAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as HttpAccount;
            if (_currentAccount == null)
                return;

            var httpAccounts = _currentSettingsProvider.Settings.ApplicationSettings.Accounts.HttpAccounts;
            if (!httpAccounts.Contains(_currentAccount))
                return;

            var interaction = new HttpAccountInteraction(_currentAccount.Copy(), Translation.EditHttpAccount);
            _interactionRequest.Raise(interaction, UpdateHttpAccountsCallback);
        }

        private void UpdateHttpAccountsCallback(HttpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            interaction.HttpAccount.CopyTo(_currentAccount);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
