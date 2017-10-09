using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class HttpAccountRemoveCommand : TranslatableCommandBase<HttpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ObservableCollection<HttpAccount> _httpAccounts;
        private readonly ObservableCollection<ConversionProfile> _profiles;
        private HttpAccount _currentAccount;
        private List<ConversionProfile> _usedInProfilesList;

        public HttpAccountRemoveCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _httpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.HttpAccounts;
            _profiles = currentSettingsProvider?.Profiles;

            _httpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.HttpAccounts ?? new ObservableCollection<HttpAccount>();
            _httpAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _httpAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as HttpAccount;
            if (_currentAccount == null)
                return;

            _usedInProfilesList = _profiles.Where(p => p.HttpSettings.AccountId.Equals(_currentAccount.AccountId)).ToList();

            var title = Translation.RemoveHttpAccount;

            var messageSb = new StringBuilder();
            messageSb.AppendLine(_currentAccount.AccountInfo);
            messageSb.AppendLine(Translation.SureYouWantToDeleteAccount);

            if (_usedInProfilesList.Count > 0)
            {
                messageSb.AppendLine();
                messageSb.AppendLine(Translation.GetAccountIsUsedInFollowingMessage(_usedInProfilesList.Count));
                messageSb.AppendLine();
                foreach (var profile in _usedInProfilesList)
                {
                    messageSb.AppendLine(profile.Name);
                }
                messageSb.AppendLine();
                messageSb.AppendLine(Translation.GetHttppGetsDisabledMessage(_usedInProfilesList.Count));
            }
            var message = messageSb.ToString();
            var icon = _usedInProfilesList.Count > 0 ? MessageIcon.Warning : MessageIcon.Question;
            var interaction = new MessageInteraction(message, title, MessageOptions.YesNo, icon);
            _interactionRequest.Raise(interaction, DeleteAccountCallback);
        }

        private void DeleteAccountCallback(MessageInteraction interaction)
        {
            if (interaction.Response != MessageResponse.Yes)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }
            if (_httpAccounts.Contains(_currentAccount))
                _httpAccounts.Remove(_currentAccount);

            foreach (var profile in _usedInProfilesList)
            {
                profile.HttpSettings.AccountId = "";
                profile.HttpSettings.Enabled = false;
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
