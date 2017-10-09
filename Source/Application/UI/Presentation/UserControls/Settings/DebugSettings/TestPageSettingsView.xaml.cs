using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for TestPageSettingsView.xaml
    /// </summary>
    public partial class TestPageSettingsView : UserControl
    {
        public TestPageSettingsView(TestPageSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
