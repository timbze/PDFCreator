using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for PrintJobView.xaml
    /// </summary>
    public partial class PrintJobView : UserControl
    {
        public PrintJobView(PrintJobViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
