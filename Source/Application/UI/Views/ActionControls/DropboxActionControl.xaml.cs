using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    /// <summary>
    /// Interaction logic for DropboxActionControl.xaml
    /// </summary>
    public partial class DropboxActionControl : ActionControl
    {
        public DropboxActionControl(DropboxActionViewModel dropboxActionViewModel) : base(dropboxActionViewModel)
        {
            InitializeComponent();
        }
    }
}
