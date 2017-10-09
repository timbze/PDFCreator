using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public partial class BackgroundUserControl : UserControl
    {
        public BackgroundUserControl(BackgroundUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
