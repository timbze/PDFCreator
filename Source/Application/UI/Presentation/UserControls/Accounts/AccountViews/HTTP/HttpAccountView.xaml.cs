using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for HttpAccountView.xaml
    /// </summary>
    public partial class HttpAccountView : UserControl
    {
        private readonly HttpAccountViewModel _viewModel;

        public HttpAccountView(HttpAccountViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.SetPasswordAction = SetPassword;
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void HttpServerPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = HttpServerPasswordBox.Password;
        }

        private void SetPassword(string password)
        {
            HttpServerPasswordBox.Password = password;
        }
    }
}
