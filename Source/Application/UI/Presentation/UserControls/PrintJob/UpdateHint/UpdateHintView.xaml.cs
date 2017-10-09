using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint
{
    public partial class UpdateHintView : UserControl
    {
        private readonly UpdateHintViewModel _viewModel;

        public UpdateHintView(UpdateHintViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
