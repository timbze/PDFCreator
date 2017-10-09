using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class ConvertTextView : UserControl
    {
        public ConvertTextView(ConvertTextViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
