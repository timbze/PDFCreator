using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public partial class CoverUserControl : UserControl
    {
        public CoverUserControl(CoverUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
