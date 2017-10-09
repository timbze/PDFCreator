using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class OutputFormatTab : UserControl
    {
        public OutputFormatTab(ConvertTabViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
