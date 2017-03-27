using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class PasswordWindow : Window
    {
        private readonly PasswordViewModel _viewModel;

        public PasswordWindow(PasswordViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            _viewModel.SetPasswordAction = SetPassword;
        }

        private void SetPassword(string password)
        {
            PasswordBox.Password = password;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}