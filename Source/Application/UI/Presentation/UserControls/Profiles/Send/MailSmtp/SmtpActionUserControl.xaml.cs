using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp
{
    public partial class SmtpActionUserControl : UserControl
    {
        public SmtpActionUserControl(SmtpActionViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
