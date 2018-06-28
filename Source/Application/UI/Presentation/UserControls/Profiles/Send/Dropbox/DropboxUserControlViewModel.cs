using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox
{
    public class DropboxUserControlViewModel : ProfileUserControlViewModel<DropboxTranslation>
    {
        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }
        public IMacroCommand AddDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts { get; set; }

        public DropboxUserControlViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
            : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            AddDropboxAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DropboxAccountAddCommand>()
                .AddCommand(new DelegateCommand(SelectNewAccountInView))
                .Build();

            DropboxAccounts = currentSettingsProvider?.Settings?.ApplicationSettings.Accounts.DropboxAccounts;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            SharedFolderTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.DropboxSettings.SharedFolder)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                .Build();
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
    }
}
