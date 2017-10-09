using System.Windows.Controls;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    /// <summary>
    ///     Interaction logic for LicenseSettingsView.xaml
    /// </summary>
    ///
    [ViewSortHint("100")]
    public partial class LicenseSettingsView : UserControl
    {
        public LicenseSettingsView(LicenseSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
