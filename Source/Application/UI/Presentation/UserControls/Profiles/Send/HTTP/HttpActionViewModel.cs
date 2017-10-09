using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP
{
    public class HttpActionViewModel : ProfileUserControlViewModel<HttpTranslation>
    {
        public ICollectionView HttpAccountsView { get; }

        private ObservableCollection<HttpAccount> _httpAccounts;
        public IMacroCommand EditAccountCommand { get; }
        public IMacroCommand AddAccountCommand { get; }

        public HttpActionViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator)
            : base(translationUpdater, currentSettingsProvider)
        {
            if (currentSettingsProvider?.Settings != null)
            {
                _httpAccounts = currentSettingsProvider.Settings.ApplicationSettings.Accounts.HttpAccounts;
                HttpAccountsView = new ListCollectionView(_httpAccounts);
                HttpAccountsView.SortDescriptions.Add(new SortDescription(nameof(HttpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            AddAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<HttpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()));

            EditAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<HttpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()));
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _httpAccounts.Last();
            HttpAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            HttpAccountsView.Refresh();
        }
    }
}
