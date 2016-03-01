using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class SmtpPasswordWindow : Window
    {
        public SmtpPasswordViewModel SmtpPasswordViewModel
        {
            get
            {
                return (SmtpPasswordViewModel)DataContext;
            }
        }

        public string SmtpPassword
        {
            get
            {
                return SmtpPasswordViewModel.SmtpPassword;
            }
            set
            {
                SmtpPasswordViewModel.SmtpPassword = value;
                SmtpPasswordBox.Password = value;
            }
        }

        public string Message;

        public SmtpPasswordResponse Response
        {
            get { return SmtpPasswordViewModel.Response; }
        }

        public SmtpPasswordWindow(SmtpPasswordMiddleButton middleButton)
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            
            InitializeComponent();

            MailOverviewPanel.Visibility = Visibility.Collapsed;
            
            switch (middleButton)
            {
                case SmtpPasswordMiddleButton.Remove:
                    SkipButton.Visibility = Visibility.Collapsed;
                    break;
                case SmtpPasswordMiddleButton.Skip:
                    RemoveButton.Visibility = Visibility.Collapsed;
                    PasswordHintText.Visibility = Visibility.Collapsed;
                    break;
                case SmtpPasswordMiddleButton.None:
                    RemoveButton.Visibility = Visibility.Collapsed;
                    SkipButton.Visibility = Visibility.Collapsed;
                    PasswordHintText.Visibility = Visibility.Collapsed;
                    Grid.SetColumn(OkButton, 1);
                    break;
                default:
                    SkipButton.Visibility = Visibility.Collapsed;
                    break;
            }

            SmtpPasswordViewModel.CloseViewAction = delegate(bool? result) { DialogResult = result; };
        }

        public SmtpPasswordWindow(SmtpPasswordMiddleButton middleButton, string emailAddress, string recipients)
            : this(middleButton)
        {
            MailOverviewPanel.Visibility = Visibility.Visible;
            EmailAddressLabel.Content = emailAddress;
            RecipientsLabel.Content = recipients;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            SmtpPasswordViewModel.SmtpPassword = SmtpPasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
            //overwrites default passwort hint text
            if (!string.IsNullOrEmpty(Message))
                PasswordHintText.Text = Message;
        }

        public SmtpPasswordResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }

    public enum SmtpPasswordMiddleButton
    {
        Remove,
        Skip,
        None
    }
}

