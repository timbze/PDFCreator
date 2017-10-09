using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public partial class SignUserControl : UserControl
    {
        public SignUserControl(SignUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
