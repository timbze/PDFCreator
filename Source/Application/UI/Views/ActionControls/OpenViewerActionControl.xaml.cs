using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class OpenViewerActionControl : ActionControl
    {
        public OpenViewerActionControl(OpenViewerActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            viewModel.Translator.Translate(this);
        }
    }
}