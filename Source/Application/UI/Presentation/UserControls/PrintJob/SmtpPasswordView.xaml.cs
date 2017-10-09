using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for SmtpPasswordView.xaml
    /// </summary>
    public partial class SmtpPasswordView : UserControl
    {
        private readonly SmtpJobStepPasswordViewModel _viewModel;

        public SmtpPasswordView(SmtpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();
        }

        private void PwbSmtpAccountChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = SmtpAccountPasswordBox.Password;
        }
    }
}
