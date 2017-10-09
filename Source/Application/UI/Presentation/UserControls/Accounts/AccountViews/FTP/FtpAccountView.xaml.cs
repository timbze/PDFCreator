using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for FtpAccountView.xaml
    /// </summary>
    public partial class FtpAccountView : UserControl
    {
        private readonly FtpAccountViewModel _viewModel;

        public FtpAccountView(FtpAccountViewModel vm)
        {
            _viewModel = vm;
            _viewModel.SetPasswordAction = SetPassword;
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void ServerPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = ServerPasswordBox.Password;
        }

        private void SetPassword(string password)
        {
            ServerPasswordBox.Password = password;
        }
    }
}
