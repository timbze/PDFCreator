using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public partial class StoreLicenseForAllUsersControl : UserControl
    {
        public StoreLicenseForAllUsersControl(StoreLicenseForAllUsersWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
