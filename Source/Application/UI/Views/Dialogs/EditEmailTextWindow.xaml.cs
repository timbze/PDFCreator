using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class EditEmailTextWindow : Window
    {
        public EditEmailTextWindow(EditEmailTextViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}