using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class ConvertTiffView : UserControl
    {
        public ConvertTiffView(ConvertTiffViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
