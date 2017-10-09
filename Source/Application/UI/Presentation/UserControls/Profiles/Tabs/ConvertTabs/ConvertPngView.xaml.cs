using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class ConvertPngView : UserControl
    {
        public ConvertPngView(ConvertPngViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
