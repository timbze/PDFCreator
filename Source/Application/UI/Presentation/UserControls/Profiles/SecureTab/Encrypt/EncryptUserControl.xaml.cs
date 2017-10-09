using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt
{
    public partial class EncryptUserControl : UserControl
    {
        public EncryptUserControl(EncryptUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
