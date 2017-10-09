using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password
{
    public partial class PasswordOverlay : UserControl
    {
        private readonly PasswordOverlayViewModel _overlayViewModel;

        public PasswordOverlay(PasswordOverlayViewModel overlayViewModel)
        {
            _overlayViewModel = overlayViewModel;
            DataContext = overlayViewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            _overlayViewModel.SetPasswordAction = SetPassword;
        }

        private void SetPassword(string password)
        {
            PasswordBox.Password = password;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _overlayViewModel.Password = PasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
