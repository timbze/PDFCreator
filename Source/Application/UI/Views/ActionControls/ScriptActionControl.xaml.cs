using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class ScriptActionControl
    {
        public ScriptActionControl(ScriptActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            viewModel.Translator.Translate(this);
        }
    }
}