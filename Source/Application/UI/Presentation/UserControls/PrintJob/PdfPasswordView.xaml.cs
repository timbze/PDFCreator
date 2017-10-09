using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class PdfPasswordView : UserControl
    {
        public PdfPasswordView(PdfJobStepPasswordViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            _viewModel.SetPasswordsInUi = SetPasswords;
        }

        private readonly PdfJobStepPasswordViewModel _viewModel;

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
    }
}
