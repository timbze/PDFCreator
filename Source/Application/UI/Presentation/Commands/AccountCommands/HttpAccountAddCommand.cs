using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class HttpAccountAddCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        public HttpAccountAddCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
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
            var newAccount = new HttpAccount();
            newAccount.AccountId = Guid.NewGuid().ToString();

            var interaction = new HttpAccountInteraction(newAccount, Translation.AddHttpAccount);
            _interactionRequest.Raise(interaction, AddHttpAccountCallback);
        }

        private void AddHttpAccountCallback(HttpAccountInteraction interaction)
        {
            if (!interaction.Success)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            var httpAccounts = _currentSettingsProvider.Settings.ApplicationSettings.Accounts.HttpAccounts;
            httpAccounts.Add(interaction.HttpAccount);

            var collectionView = CollectionViewSource.GetDefaultView(httpAccounts);
            collectionView.MoveCurrentTo(interaction.HttpAccount);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
