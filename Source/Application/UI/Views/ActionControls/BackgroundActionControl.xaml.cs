using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class BackgroundActionControl : ActionControl
    {
        public BackgroundActionControl(BackgroundActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            viewModel?.Translator.Translate(this);
        }
    }
}