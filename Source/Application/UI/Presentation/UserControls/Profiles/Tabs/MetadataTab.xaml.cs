using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public partial class MetadataTab : UserControl
    {
        public MetadataTab(MetadataViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
