using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class TimeServerPasswordWindow : Window
    {
        public TimeServerPasswordViewModel TimeServerPasswordViewModel
        {
            get
            {
                return (TimeServerPasswordViewModel)DataContext;
            }
        }

        public string TimeServerLoginName
        {
            get
            {
                return TimeServerPasswordViewModel.TimeServerLoginName;
            }
            set
            {
                TimeServerPasswordViewModel.TimeServerLoginName = value;
                TimeServerLoginNameBox.Text = value;
            }
        }

        public string TimeServerPassword
        {
            get
            {
                return TimeServerPasswordViewModel.TimeServerPassword;
            }
            set
            {
                TimeServerPasswordViewModel.TimeServerPassword = value;
                TimeServerPasswordBox.Password = value;
            }
        }

        public TimeServerPasswordResponse Response
        {
            get { return TimeServerPasswordViewModel.Response; }
        }

        public TimeServerPasswordWindow()
        {
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            
            InitializeComponent();

            TimeServerPasswordViewModel.CloseViewAction = delegate(bool? result) { DialogResult = result; };
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            TimeServerPasswordViewModel.TimeServerPassword = TimeServerPasswordBox.Password;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public TimeServerPasswordResponse ShowDialogTopMost()
        {
            TopMostHelper.ShowDialogTopMost(this, false);
            return Response;
        }
    }
}
