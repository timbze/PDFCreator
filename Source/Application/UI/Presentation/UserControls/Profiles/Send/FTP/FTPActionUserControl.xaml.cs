using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP
{
    public partial class FTPActionUserControl : UserControl
    {
        public FTPActionUserControl(FtpActionViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
