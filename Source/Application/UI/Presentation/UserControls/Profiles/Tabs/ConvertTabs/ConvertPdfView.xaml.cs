using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public partial class ConvertPdfView : UserControl
    {
        public ConvertPdfView(ConvertPdfViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        // This can stay in the code behind, can't easily be done in viewmodel because the textbox is bound to a double
        private void JpegFactorTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var cursorPosition = JpegFactorTextBox.SelectionStart;
            JpegFactorTextBox.Text = JpegFactorTextBox.Text.Replace(',', '.');
            JpegFactorTextBox.SelectionStart = cursorPosition;
        }
    }
}
