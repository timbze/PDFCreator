using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
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
    public class DropboxAccountRemoveCommand : TranslatableCommandBase<DropboxTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IDropboxService _dropboxService;
        private readonly ObservableCollection<DropboxAccount> _dropboxAccounts;
        private readonly ObservableCollection<ConversionProfile> _profiles;
        private DropboxAccount _currentAccount;
        private List<ConversionProfile> _usedInProfilesList;

        public DropboxAccountRemoveCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, IDropboxService dropboxService, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _dropboxService = dropboxService;
            _profiles = currentSettingsProvider?.Profiles;

            _dropboxAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.DropboxAccounts ?? new ObservableCollection<DropboxAccount>();
            _dropboxAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _dropboxAccounts?.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as DropboxAccount;
            if (_currentAccount == null)
                return;

            _usedInProfilesList = _profiles.Where(p => p.DropboxSettings.AccountId.Equals(_currentAccount.AccountId)).ToList();

            var title = Translation.RemoveDropboxAccount;

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
                messageSb.AppendLine(Translation.GetDropboxGetsDisabledMessage(_usedInProfilesList.Count));
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

            if (_dropboxAccounts.Contains(_currentAccount))
                _dropboxAccounts.Remove(_currentAccount);

            foreach (var profile in _usedInProfilesList)
            {
                profile.DropboxSettings.AccountId = "";
                profile.DropboxSettings.Enabled = false;
            }

            try
            {
                _dropboxService.RevokeToken(_currentAccount.AccessToken);
            }
            catch (Exception)
            {
                // ignored
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
