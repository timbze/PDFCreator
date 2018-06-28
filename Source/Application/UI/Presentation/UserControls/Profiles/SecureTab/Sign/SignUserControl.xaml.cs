using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public partial class SignUserControl : UserControl
    {
        private readonly SignUserControlViewModel _viewModel;

        public SignUserControl(SignUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.Password))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        CertificatePasswordBox.Password = _viewModel.Password;
                    });
                }
            };
        }

        private void PwbSignaturePasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = CertificatePasswordBox.Password;
        }
    }
}
