using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    /// <summary>
    ///     Interaction logic for OfflineActivationWindow.xaml
    /// </summary>
    public partial class OfflineActivationUserControl : UserControl
    {
        public OfflineActivationUserControl(OfflineActivationUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
           
        }
    }
}