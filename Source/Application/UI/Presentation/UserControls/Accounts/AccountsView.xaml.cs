using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public partial class AccountsView : UserControl
    {
        public AccountsView(AccountsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
