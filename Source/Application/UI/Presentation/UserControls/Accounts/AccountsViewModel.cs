using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

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

            FtpAccountAddCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountAddCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            FtpAccountRemoveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountRemoveCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            FtpAccountEditCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountEditCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            SmtpAccountAddCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountAddCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            SmtpAccountRemoveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountRemoveCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            SmtpAccountEditCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SmtpAccountEditCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            HttpAccountAddCommand = commandLocator.CreateMacroCommand()
                .AddCommand<HttpAccountAddCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            HttpAccountRemoveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<HttpAccountRemoveCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            HttpAccountEditCommand = commandLocator.CreateMacroCommand()
                .AddCommand<HttpAccountEditCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            DropboxAccountAddCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DropboxAccountAddCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            DropboxAccountRemoveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DropboxAccountRemoveCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            TimeServerAccountAddCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            TimeServerAccountRemoveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountRemoveCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

            TimeServerAccountEditCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountEditCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .Build();

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

        public IMacroCommand FtpAccountAddCommand { get; }
        public IMacroCommand FtpAccountRemoveCommand { get; }
        public IMacroCommand FtpAccountEditCommand { get; }

        public IMacroCommand SmtpAccountAddCommand { get; }
        public IMacroCommand SmtpAccountRemoveCommand { get; }
        public IMacroCommand SmtpAccountEditCommand { get; }

        public IMacroCommand HttpAccountAddCommand { get; }
        public IMacroCommand HttpAccountRemoveCommand { get; }
        public IMacroCommand HttpAccountEditCommand { get; }

        public IMacroCommand DropboxAccountAddCommand { get; }
        public IMacroCommand DropboxAccountRemoveCommand { get; }

        public IMacroCommand TimeServerAccountAddCommand { get; }
        public IMacroCommand TimeServerAccountRemoveCommand { get; }
        public IMacroCommand TimeServerAccountEditCommand { get; }
    }
}
