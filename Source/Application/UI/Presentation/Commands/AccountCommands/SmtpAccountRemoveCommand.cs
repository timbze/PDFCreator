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
    public class SmtpAccountRemoveCommand : TranslatableCommandBase<SmtpTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ObservableCollection<SmtpAccount> _smtpAccounts;
        private readonly ObservableCollection<ConversionProfile> _profiles;
        private SmtpAccount _currentAccount;
        private List<ConversionProfile> _usedInProfilesList;

        public SmtpAccountRemoveCommand(IInteractionRequest interactionRequest, ICurrentSettingsProvider currentSettingsProvider, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _smtpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.SmtpAccounts;
            _profiles = currentSettingsProvider?.Profiles;

            _smtpAccounts = currentSettingsProvider?.Settings?.ApplicationSettings?.Accounts?.SmtpAccounts ?? new ObservableCollection<SmtpAccount>();
            _smtpAccounts.CollectionChanged += (sender, args) => RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _smtpAccounts.Count > 0;
        }

        public override void Execute(object parameter)
        {
            _currentAccount = parameter as SmtpAccount;
            if (_currentAccount == null)
                return;

            _usedInProfilesList = _profiles.Where(p => p.EmailSmtpSettings.AccountId.Equals(_currentAccount.AccountId)).ToList();

            var title = Translation.RemoveSmtpAccount;

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
                messageSb.AppendLine(Translation.GetSmtpGetsDisabledMessage(_usedInProfilesList.Count));
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
            if (_smtpAccounts.Contains(_currentAccount))
                _smtpAccounts.Remove(_currentAccount);

            foreach (var profile in _usedInProfilesList)
            {
                profile.EmailSmtpSettings.AccountId = "";
                profile.EmailSmtpSettings.Enabled = false;
            }

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
