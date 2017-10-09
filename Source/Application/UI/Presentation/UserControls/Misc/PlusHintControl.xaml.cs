using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class PlusHintControl : UserControl
    {
        public PlusHintControl(PlusHintViewModel plusHintViewModel)
        {
            DataContext = plusHintViewModel;
            InitializeComponent();
        }
    }
}
