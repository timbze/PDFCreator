using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared
{
    /// <summary>
    ///     Interaction logic for SettingControlsView.xaml
    /// </summary>
    public partial class SettingControlsView : UserControl
    {
        public SettingControlsView(SettingControlsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
