using System.Windows;
using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class SignaturePasswordWindow
    {
        private readonly SignaturePasswordViewModel _viewModel;

        public SignaturePasswordWindow(SignaturePasswordViewModel viewModel, ITranslator translator)
        {
            _viewModel = viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            DataContext = viewModel;

            InitializeComponent();

            translator.Translate(this);

            // As password boxes do not allow bindings, we need to update it manually, when the password was set in the interaction
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Password))
                    PasswordBox.Password = _viewModel.Password;
            };
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }
    }
}