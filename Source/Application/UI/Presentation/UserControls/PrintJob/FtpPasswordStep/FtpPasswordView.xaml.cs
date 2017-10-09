using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for FtpPasswordView.xaml
    /// </summary>
    public partial class FtpPasswordView : UserControl
    {
        private readonly FtpJobStepPasswordViewModel _viewModel;

        public FtpPasswordView(FtpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();
        }

        private void PwbFtpAccountChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = FtpAccountPasswordBox.Password;
        }
    }
}
