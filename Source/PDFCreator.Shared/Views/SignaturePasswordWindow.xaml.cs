using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class SignaturePasswordWindow
    {
        public SignaturePasswordWindow(PasswordMiddleButton middleButton, string certificateFile)
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();

            if (middleButton == PasswordMiddleButton.Skip)
            {
                RemoveButton.Visibility = Visibility.Collapsed;
                PasswordHintText.Visibility = Visibility.Collapsed;
            }
            else
            {
                SkipButton.Visibility = Visibility.Collapsed;
                PasswordHintText.Visibility = Visibility.Visible;
            }

            SignaturePasswordViewModel.SignaturePasswordCheck = new SignaturePasswordCheck(certificateFile);
            SignaturePasswordViewModel.CloseViewAction = delegate(bool? result) { DialogResult = result; };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public SignaturePasswordViewModel SignaturePasswordViewModel
         {
            get
            {
                return (SignaturePasswordViewModel) DataContext;
            }
        }

        public string Password
        {
            get
            {
                return SignaturePasswordViewModel.Password;
            }
            set
            {
                SignaturePasswordViewModel.Password = value;
                PasswordBox.Password = value;
            }
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            SignaturePasswordViewModel.Password = PasswordBox.Password;
        }
    }

    public enum PasswordMiddleButton
    {
        Skip,
        Remove
    }
}
