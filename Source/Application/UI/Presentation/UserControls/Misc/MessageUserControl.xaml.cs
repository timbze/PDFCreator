using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class MessageUserControl : UserControl
    {
        public MessageUserControl(MessageUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Clipboard.SetText(MessageText.Text);
        }
    }
}
