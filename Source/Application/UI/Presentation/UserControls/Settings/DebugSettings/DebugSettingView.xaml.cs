using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public partial class DebugSettingView : UserControl
    {
        public DebugSettingView(DebugSettingsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
