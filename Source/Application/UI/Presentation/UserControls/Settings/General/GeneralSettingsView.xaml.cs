using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView(GeneralSettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
