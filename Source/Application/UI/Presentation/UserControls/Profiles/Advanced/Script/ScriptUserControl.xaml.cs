using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script
{
    public partial class ScriptUserControl : UserControl
    {
        public ScriptUserControl(ScriptUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
