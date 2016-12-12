using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(MessageWindowViewModel viewModel)
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