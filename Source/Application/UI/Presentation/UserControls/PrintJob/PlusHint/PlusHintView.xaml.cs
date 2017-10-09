using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint
{
    /// <summary>
    /// Interaction logic for PlusHintView.xaml
    /// </summary>
    public partial class PlusHintView : UserControl
    {
        public PlusHintView(PlusHintViewModel viewModel)
        {
            DataContext = viewModel;

            //Todo: Reuired?
            //Loaded += (sender, e) =>
            //    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }
    }
}
