using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class EncryptionPasswordsWindow : Window
    {
        public EncryptionPasswordViewModelwithRemove EncryptionPasswordViewModel
        {
            get
            {
                return (EncryptionPasswordViewModelwithRemove)DataContext;
            }
        }

        public EncryptionPasswordResponse Response
        {
            get { return EncryptionPasswordViewModel.Response; }
        }

        public string OwnerPassword
        {
            get
            {
                return EncryptionPasswordViewModel.OwnerPassword;
            }
            set
            {
                EncryptionPasswordViewModel.OwnerPassword = value;
                OwnerPasswordBox.Password = value;
            }
        }

        public string UserPassword
        {
            get
            {
                return EncryptionPasswordViewModel.UserPassword;
            }
            set
            {
                EncryptionPasswordViewModel.UserPassword = value;
                UserPasswordBox.Password = value;
            }
        }

        public EncryptionPasswordsWindow(EncryptionPasswordMiddleButton middleButton, bool askOwnerPassword, bool askUserPassword)
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            
            InitializeComponent();

            switch (middleButton)
            {
                case EncryptionPasswordMiddleButton.Skip:
                    DataContext = new EncryptionPasswordViewModelwithSkip(askOwnerPassword, askUserPassword);
                    RemoveButton.Visibility = Visibility.Collapsed;
                    PasswordHintText.Visibility = Visibility.Collapsed;
                    break;
                default:
                    DataContext = new EncryptionPasswordViewModelwithRemove(askUserPassword);
                    SkipButton.Visibility = Visibility.Collapsed;
                    break;
            }

            EncryptionPasswordViewModel.CloseViewAction = delegate(bool? result) { DialogResult = result; };
        }

        private void PwbOwnerPasswordChanged(object sender, RoutedEventArgs e)
        {
            EncryptionPasswordViewModel.OwnerPassword = OwnerPasswordBox.Password;
        }

        private void PwbUserPasswordChanged(object sender, RoutedEventArgs e)
        {
            EncryptionPasswordViewModel.UserPassword = UserPasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public EncryptionPasswordResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }

    public enum EncryptionPasswordMiddleButton
    {
        Skip,
        Remove
    }
}
