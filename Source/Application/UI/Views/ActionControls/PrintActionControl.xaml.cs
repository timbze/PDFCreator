using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class PrintActionControl : ActionControl
    {
        public PrintActionControl(PrintActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}