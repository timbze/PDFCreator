using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for TimeServerAccountView.xaml
    /// </summary>
    public partial class TimeServerAccountView : UserControl
    {
        private readonly TimeServerAccountViewModel _viewModel;

        public TimeServerAccountView(TimeServerAccountViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.SetPasswordAction = SetPassword;
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void TimeServerPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = TimeServerPasswordBox.Password;
        }

        private void SetPassword(string password)
        {
            TimeServerPasswordBox.Password = password;
        }
    }
}
