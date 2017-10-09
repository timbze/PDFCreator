using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public partial class LicenseUpdateControl : UserControl
    {
        public LicenseUpdateControl(LicenseSettingsView view)
        {
            InitializeComponent();
            PartContent.Children.Add(view);
        }
    }
}
