using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.UserToken
{
    public partial class UserTokenUserControl : UserControl
    {
        public UserTokenUserControl(UserTokenUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
