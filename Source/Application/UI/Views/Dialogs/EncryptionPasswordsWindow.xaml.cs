using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class EncryptionPasswordsWindow : Window
    {
        private readonly EncryptionPasswordViewModel _viewModel;

        public EncryptionPasswordsWindow(EncryptionPasswordViewModel viewModel)
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