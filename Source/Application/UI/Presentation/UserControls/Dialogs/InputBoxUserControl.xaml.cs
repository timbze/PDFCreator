namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs
{
    public partial class InputBoxUserControl
    {
        public InputBoxUserControl(InputBoxWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
