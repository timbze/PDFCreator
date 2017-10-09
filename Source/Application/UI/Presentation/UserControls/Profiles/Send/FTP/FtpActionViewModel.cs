using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP
{
    public class FtpActionViewModel : ProfileUserControlViewModel<FtpActionTranslation>
    {
        public ICollectionView FtpAccountsView { get; }

        private readonly ObservableCollection<FtpAccount> _ftpAccounts;

        public IMacroCommand EditAccountCommand { get; }
        public IMacroCommand AddAccountCommand { get; }

        public TokenViewModel DirectoryTokenViewModel { get; set; }

        private readonly TokenReplacer _tokenReplacer;

        public FtpActionViewModel(TokenHelper tokenHelper, ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator)
            : base(translationUpdater, currentSettingsProvider)
        {
            if (tokenHelper != null)
            {
                _tokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                DirectoryTokenViewModel = new TokenViewModel(x => CurrentProfile.Ftp.Directory = x, () => CurrentProfile?.Ftp.Directory, _tokenReplacer.GetTokenNames(true), ReplaceTokens);
            }

            if (currentSettingsProvider?.Settings != null)
            {
                _ftpAccounts = currentSettingsProvider.Settings.ApplicationSettings.Accounts.FtpAccounts;
                FtpAccountsView = new ListCollectionView(_ftpAccounts);
                FtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(FtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            AddAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<FtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()));

            EditAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<FtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()));
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _ftpAccounts.Last();
            FtpAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            FtpAccountsView.Refresh();
        }

        private string ReplaceTokens(string s)
        {
            if (s == null)
                return string.Empty;

            return _tokenReplacer.ReplaceTokens(s);
        }
    }
}
