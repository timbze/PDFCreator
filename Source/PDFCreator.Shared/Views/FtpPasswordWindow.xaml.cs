using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class FtpPasswordWindow : Window
    {
        public FtpPasswordViewModel FtpPasswordViewModel
        {
            get
            {
                return (FtpPasswordViewModel) DataContext;
            } 
        }

        public string FtpPassword
        {
            get
            {
                return FtpPasswordViewModel.FtpPassword;
            }
            set
            {
                FtpPasswordViewModel.FtpPassword = value;
                FtpPasswordBox.Password = value;
            }
        }

        public FtpPasswordResponse Response
        {
            get { return FtpPasswordViewModel.Response; }
        }

        public FtpPasswordWindow(FtpPasswordMiddleButton middleButton)
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            
            InitializeComponent();

            switch (middleButton)
            {
                case FtpPasswordMiddleButton.Skip:
                    RemoveButton.Visibility = Visibility.Collapsed;
                    PasswordHintText.Visibility = Visibility.Collapsed;
                    break;
                default:
                    SkipButton.Visibility = Visibility.Collapsed;
                    break;
            }

            FtpPasswordViewModel.CloseViewAction = delegate(bool? result) { DialogResult = result; };
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            FtpPasswordViewModel.FtpPassword = FtpPasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public FtpPasswordResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }

    public enum FtpPasswordMiddleButton
    {
        Remove,
        Skip
    }
}
