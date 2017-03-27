using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class DefectiveProfilesWindow : Window
    {
        public DefectiveProfilesWindow(DefectiveProfilesWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var errorText = ((DefectiveProfilesWindowViewModel) DataContext).ComposeCopyAndPasteText();
            Clipboard.SetText(errorText);
        }
    }
}