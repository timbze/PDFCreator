using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class DropboxActionViewModel : ActionViewModel
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private SynchronizedCollection<DropboxAccount> _accountsCollection;

        public DropboxActionViewModel(ITranslator translator, IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
            AddDropboxAccountCommand = new DelegateCommand(AuthoriseDropboxUser);
            _accountsCollection = new SynchronizedCollection<DropboxAccount>(new List<DropboxAccount>());
            RemoveDropboxAccountCommand = new DelegateCommand(RemoveDropboxAccount, RemoveDropboxCanExecute);
            var tokenHelper = new TokenHelper(translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            TokenViewModel = new TokenViewModel(x => CurrentProfile.DropboxSettings.SharedFolder = x, () => CurrentProfile?.DropboxSettings.SharedFolder,
                tokenHelper.GetTokenListForDirectory());
            DisplayName = translator.GetTranslation("DropboxActionSettings", "DisplayName");
        }

        private bool RemoveDropboxCanExecute(object o)
        {
            return DropboxAccounts.Any();
        }

        public override bool IsEnabled
        {
            get { return (CurrentProfile != null) && CurrentProfile.DropboxSettings.Enabled; }
            set
            {
                CurrentProfile.DropboxSettings.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public TokenReplacer TokenReplacer { get; set; }

        public TokenViewModel TokenViewModel { get; set; }

        public DelegateCommand AddDropboxAccountCommand { get; set; }
        public DelegateCommand RemoveDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts => _accountsCollection.ObservableCollection;

        private void RemoveDropboxAccount(object obj)
        {
            var currentSelectedAccount = DropboxAccounts.FirstOrDefault(item => item.AccountId == CurrentProfile.DropboxSettings.AccountId);
            if (currentSelectedAccount != null)
            {
                DropboxAccounts.Remove(currentSelectedAccount);
                var account = Accounts.DropboxAccounts.FirstOrDefault();
                CurrentProfile.DropboxSettings.AccountId = account == null ? "" : account.AccountId;
                RaisePropertyChanged(nameof(Accounts));
            }

            var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
            collectionView.MoveCurrentToLast();
        }


        protected override void SetAccount(Accounts accounts)
        {
            _accountsCollection = new SynchronizedCollection<DropboxAccount>(Accounts.DropboxAccounts);
            _accountsCollection.ObservableCollection.CollectionChanged += (sender, args) => RemoveDropboxAccountCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(DropboxAccounts));
            RemoveDropboxAccountCommand.RaiseCanExecuteChanged();
        }

        protected override void HandleCurrentProfileChanged()
        {
            TokenViewModel.RaiseTextChanged();
        }

        /// <summary>
        ///     Calling dropbox authorisation api
        /// </summary>
        /// <param name="obj"></param>
        private void AuthoriseDropboxUser(object obj)
        {
            var interaction = new DropboxInteraction();

            _interactionInvoker.Invoke(interaction);
            if (interaction.Success)
            {
                if (DropboxAccounts.Any(item => item.AccountId.Equals(interaction.AccountId)) == false)
                    DropboxAccounts.Add(new DropboxAccount
                    {
                        AccessToken = interaction.AccessToken,
                        AccountId = interaction.AccountId,
                        AccountInfo = interaction.AccountInfo
                    });

                CurrentProfile.DropboxSettings.AccountId = interaction.AccountId;
                var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
                collectionView.MoveCurrentToLast();
            }
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.Dropbox;
        }
    }
}
