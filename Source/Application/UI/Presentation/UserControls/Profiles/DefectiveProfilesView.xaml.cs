using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class DefectiveProfilesView : UserControl
    {
        public DefectiveProfilesView(DefectiveProfilesViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        private void CopyCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var errorText = ((DefectiveProfilesViewModel)DataContext).ComposeCopyAndPasteText();
            Clipboard.SetText(errorText);
        }
    }
}
