using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class UpdateAvailableWindow : Window
    {
        public UpdateAvailableWindow(UpdateAvailableViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            viewModel.Translator.Translate(this);
        }
    }
}