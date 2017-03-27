using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow(InputBoxWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}