using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class ConvertJpgView : UserControl
    {
        public ConvertJpgView(ConvertJpgViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
