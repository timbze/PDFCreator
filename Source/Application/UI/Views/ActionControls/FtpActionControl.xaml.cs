using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public partial class FtpActionControl : ActionControl
    {
        public FtpActionControl(FtpActionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            viewModel.Translator.Translate(this);
        }
    }
}