using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class AttachmentActionControl : ActionControl
    {
        public AttachmentActionControl(AttatchmentActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            viewModel?.Translator.Translate(this);
        }
    }
}