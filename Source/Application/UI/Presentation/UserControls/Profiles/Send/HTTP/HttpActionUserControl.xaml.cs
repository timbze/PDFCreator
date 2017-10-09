using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP
{
    public partial class HttpActionUserControl : UserControl
    {
        public HttpActionUserControl(HttpActionViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
