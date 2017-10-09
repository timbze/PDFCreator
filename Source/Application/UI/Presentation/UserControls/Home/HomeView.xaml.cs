using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public partial class HomeView : UserControl
    {
        public HomeView(HomeViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
