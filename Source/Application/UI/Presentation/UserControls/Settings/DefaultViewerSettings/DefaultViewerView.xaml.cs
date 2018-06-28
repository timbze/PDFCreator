using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings
{
    public partial class DefaultViewerView : UserControl
    {
        public DefaultViewerView(DefaultViewerViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
