using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class SignaturePasswordView : UserControl
    {
        private readonly IPasswordButtonViewModel _viewModel;

        public SignaturePasswordView(IPasswordButtonViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            InitializeComponent();

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.Password))
                    SignatureAccountPasswordBox.Password = _viewModel.Password;
            };
        }

        private void PwbSignaturePasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = SignatureAccountPasswordBox.Password;
        }
    }

    public class SignaturePasswordOverlayView : SignaturePasswordView
    {
        public SignaturePasswordOverlayView(SignaturePasswordOverlayViewModel viewModel) : base(viewModel)
        {
        }
    }

    public class SignaturePasswordStepView : SignaturePasswordView
    {
        public SignaturePasswordStepView(SignaturePasswordStepViewModel viewModel) : base(viewModel)
        {
        }
    }
}
