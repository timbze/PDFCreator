using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public partial class AttachmentUserControl : UserControl
    {
        public AttachmentUserControl(AttachmentUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
