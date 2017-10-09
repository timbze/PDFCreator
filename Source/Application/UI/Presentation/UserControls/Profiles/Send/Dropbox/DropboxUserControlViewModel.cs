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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox
{
    public class DropboxUserControlViewModel : ProfileUserControlViewModel<DropboxTranslation>
    {
        public TokenReplacer TokenReplacer { get; set; }
        public TokenViewModel SharedFolderTokenViewModel { get; set; }
        public IMacroCommand AddDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts { get; set; }

        public DropboxUserControlViewModel(TokenHelper tokenHelper, ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator)
            : base(translationUpdater, currentSettingsProvider)
        {
            AddDropboxAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<DropboxAccountAddCommand>()
                .AddCommand(new DelegateCommand(SelectNewAccountInView));

            DropboxAccounts = currentSettingsProvider?.Settings?.ApplicationSettings.Accounts.DropboxAccounts;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                SharedFolderTokenViewModel = new TokenViewModel(x => CurrentProfile.DropboxSettings.SharedFolder = x, () => CurrentProfile?.DropboxSettings.SharedFolder, tokenHelper.GetTokenListForDirectory(), ReplaceTokens);
            }
        }

        private void SelectNewAccountInView(object obj)
        {
            var latestAccount = DropboxAccounts.Last();
            var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
            collectionView.MoveCurrentTo(latestAccount);
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            SharedFolderTokenViewModel.RaiseTextChanged();
        }

        private string ReplaceTokens(string s)
        {
            if (s != null)
            {
                return TokenReplacer.ReplaceTokens(s);
            }
            return string.Empty;
        }
    }
}
