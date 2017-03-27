using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class EmailClientActionControl : ActionControl
    {
        public EmailClientActionControl(EmailClientActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}