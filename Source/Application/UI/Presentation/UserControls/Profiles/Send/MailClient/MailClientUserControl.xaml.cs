using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient
{
    public partial class MailClientUserControl : UserControl
    {
        public MailClientUserControl(MailClientControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
