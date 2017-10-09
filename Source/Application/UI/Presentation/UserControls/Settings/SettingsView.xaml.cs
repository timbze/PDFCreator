using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    /// <summary>
    ///     Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
