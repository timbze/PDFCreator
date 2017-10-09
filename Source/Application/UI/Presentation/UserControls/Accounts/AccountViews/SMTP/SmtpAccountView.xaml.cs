using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for SmtpAccountView.xaml
    /// </summary>
    public partial class SmtpAccountView : UserControl
    {
        private readonly SmtpAccountViewModel _viewModel;

        public SmtpAccountView(SmtpAccountViewModel viewModel)
        {
            _viewModel = viewModel;
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
