using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
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

        public TokenViewModel<ConversionProfile> DirectoryTokenViewModel { get; set; }

        public FtpActionViewModel(TokenHelper tokenHelper, ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator, ITokenViewModelFactory tokenViewModelFactory)
            : base(translationUpdater, currentSettingsProvider)
        {
            // TODO update on translation change!
            DirectoryTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.Ftp.Directory)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListWithFormatting())
                .Build();

            if (currentSettingsProvider?.Settings != null)
            {
                _ftpAccounts = currentSettingsProvider.Settings.ApplicationSettings.Accounts.FtpAccounts;
                FtpAccountsView = new ListCollectionView(_ftpAccounts);
                FtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(FtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            AddAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();
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
    }
}
