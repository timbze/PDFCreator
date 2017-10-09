using System.Text;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp
{
    public partial class StampUserControl : UserControl
    {
        public StampUserControl(StampUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var caret = StampText.CaretIndex;
            var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(StampText.Text);
            StampText.Text = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            StampText.CaretIndex = caret;
        }
    }
}
