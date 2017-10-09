using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public partial class SaveTab : UserControl
    {
        public SaveTab(SaveTabViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
