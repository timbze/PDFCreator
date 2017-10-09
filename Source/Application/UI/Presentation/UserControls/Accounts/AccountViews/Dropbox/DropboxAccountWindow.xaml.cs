using System.Windows.Navigation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    ///     Interaction logic for DropboxAuthenticationWindow.xaml
    /// </summary>
    public partial class DropboxAccountWindow
    {
        private readonly DropboxAccountViewModel _viewModel;

        public DropboxAccountWindow(DropboxAccountViewModel viewModel, IWinInetHelper winInetHelper)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();
            // make uri to navigate
            var authorizeUri = viewModel.GetAuthorizeUri();
            winInetHelper.EndBrowserSession();
            Browser.Navigate(authorizeUri);
        }

        /// <summary>
        ///     Event fired when user enter his credentials into dropbox form and hits enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            _viewModel.SetAccessTokenAndUserInfo(e.Uri);
        }
    }
}
