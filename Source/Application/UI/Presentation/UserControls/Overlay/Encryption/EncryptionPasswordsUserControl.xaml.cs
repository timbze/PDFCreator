using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption
{
    public partial class EncryptionPasswordsUserControl : UserControl
    {
        private readonly EncryptionPasswordUserControlViewModel _viewModel;

        public EncryptionPasswordsUserControl(EncryptionPasswordUserControlViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            _viewModel.SetPasswordInUi = SetPasswords;
        }

        private void SetPasswords(string ownerPassword, string userPassword)
        {
            OwnerPasswordBox.Password = ownerPassword;
            UserPasswordBox.Password = userPassword;
        }

        private void PwbOwnerPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.OwnerPassword = OwnerPasswordBox.Password;
        }

        private void PwbUserPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.UserPassword = UserPasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
