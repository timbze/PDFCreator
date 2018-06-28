using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public class AccountsViewModel : TranslatableViewModelBase<AccountsTranslation>
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IDispatcher _dispatcher;
        private readonly IGpoSettings _gpoSettings;
        public CompositeCollection AllAccounts { get; private set; } = new CompositeCollection();

        private Conversion.Settings.Accounts _accounts;

        public Visibility ShowAddAccountsHint
        {
            get
            {
                var numberOfAccounts = 0;
                if (_accounts != null)
                {
                    numberOfAccounts += _accounts.SmtpAccounts.Count;
                    numberOfAccounts += _accounts.DropboxAccounts.Count;
                    numberOfAccounts += _accounts.FtpAccounts.Count;
                    numberOfAccounts += _accounts.HttpAccounts.Count;
                    numberOfAccounts += _accounts.TimeServerAccounts.Count;
                }

                return numberOfAccounts <= 4 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public AccountsViewModel(ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator, ITranslationUpdater translationUpdater, IDispatcher dispatcher, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;

            ConflateAllAccounts(_currentSettingsProvider.Settings);

            FtpAccountAddCommand = commandLocator.GetCommand<FtpAccountAddCommand>();
            FtpAccountRemoveCommand = commandLocator.GetCommand<FtpAccountRemoveCommand>();
            FtpAccountEditCommand = commandLocator.GetCommand<FtpAccountEditCommand>();

            SmtpAccountAddCommand = commandLocator.GetCommand<SmtpAccountAddCommand>();
            SmtpAccountRemoveCommand = commandLocator.GetCommand<SmtpAccountRemoveCommand>();
            SmtpAccountEditCommand = commandLocator.GetCommand<SmtpAccountEditCommand>();

            HttpAccountAddCommand = commandLocator.GetCommand<HttpAccountAddCommand>();
            HttpAccountRemoveCommand = commandLocator.GetCommand<HttpAccountRemoveCommand>();
            HttpAccountEditCommand = commandLocator.GetCommand<HttpAccountEditCommand>();

            DropboxAccountAddCommand = commandLocator.GetCommand<DropboxAccountAddCommand>();
            DropboxAccountRemoveCommand = commandLocator.GetCommand<DropboxAccountRemoveCommand>();

            TimeServerAccountAddCommand = commandLocator.GetCommand<TimeServerAccountAddCommand>();
            TimeServerAccountRemoveCommand = commandLocator.GetCommand<TimeServerAccountRemoveCommand>();
            TimeServerAccountEditCommand = commandLocator.GetCommand<TimeServerAccountEditCommand>();

            if (_currentSettingsProvider != null)
                _currentSettingsProvider.SelectedProfileChanged += CurrentSettingsProviderOnSelectedProfileChanged;
        }

        private void CurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _dispatcher.BeginInvoke(() => ConflateAllAccounts(_currentSettingsProvider.Settings));
        }

        private void ConflateAllAccounts(PdfCreatorSettings settings)
        {
            _accounts = settings?.ApplicationSettings?.Accounts;

            if (_accounts == null)
                return;

            AllAccounts.Clear();

            _accounts.SmtpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = _accounts.SmtpAccounts });

            _accounts.DropboxAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = _accounts.DropboxAccounts });

            _accounts.FtpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = _accounts.FtpAccounts });

            _accounts.HttpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = _accounts.HttpAccounts });

            _accounts.TimeServerAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = _accounts.TimeServerAccounts });

            RaisePropertyChanged(nameof(AllAccounts));
        }

        public object IsAccountsDisabled
        {
            get
            {
                if (_currentSettingsProvider.Settings.ApplicationSettings == null)
                    return false;

                return _gpoSettings != null ? _gpoSettings.DisableProfileManagement : false;
            }
        }

        private void RaiseAddAccountsBelowVisibilityChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RaisePropertyChanged(nameof(ShowAddAccountsHint));
        }

        public ICommand FtpAccountAddCommand { get; }
        public ICommand FtpAccountRemoveCommand { get; }
        public ICommand FtpAccountEditCommand { get; }

        public ICommand SmtpAccountAddCommand { get; }
        public ICommand SmtpAccountRemoveCommand { get; }
        public ICommand SmtpAccountEditCommand { get; }

        public ICommand HttpAccountAddCommand { get; }
        public ICommand HttpAccountRemoveCommand { get; }
        public ICommand HttpAccountEditCommand { get; }

        public ICommand DropboxAccountAddCommand { get; }
        public ICommand DropboxAccountRemoveCommand { get; }

        public ICommand TimeServerAccountAddCommand { get; }
        public ICommand TimeServerAccountRemoveCommand { get; }
        public ICommand TimeServerAccountEditCommand { get; }
    }
}
