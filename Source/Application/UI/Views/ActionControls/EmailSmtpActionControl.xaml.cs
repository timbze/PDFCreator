using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class EmailSmtpActionControl : ActionControl
    {
        public EmailSmtpActionControl(EmailSmtpActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}