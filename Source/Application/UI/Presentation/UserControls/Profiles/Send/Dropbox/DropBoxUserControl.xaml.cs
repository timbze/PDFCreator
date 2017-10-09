using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox
{
    public partial class DropboxUserControl : UserControl
    {
        public DropboxUserControl(DropboxUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
