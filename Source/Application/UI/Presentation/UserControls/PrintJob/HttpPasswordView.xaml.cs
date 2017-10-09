using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for HttpPasswordView.xaml
    /// </summary>
    public partial class HttpPasswordView : System.Windows.Controls.UserControl
    {
        private readonly HttpJobStepPasswordViewModel _viewModel;

        public HttpPasswordView(HttpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();
        }

        private void PwbHttpAccountChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = HttpAccountPasswordBox.Password;
        }
    }
}
