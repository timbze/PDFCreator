using System.Windows;
using System.Windows.Navigation;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

//using System.Net.Http;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{

    /// <summary>
    ///     Interaction logic for DropboxAuthenticationWindow.xaml
    /// </summary>
    public partial class DropboxAuthenticationWindow : Window
    {
        //private readonly IDropboxService _dropboxService;
        private readonly DropboxAuthenticationWindowViewModel _viewModel;

        public DropboxAuthenticationWindow(DropboxAuthenticationWindowViewModel viewModel, IWinInetHelper winInetHelper)
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