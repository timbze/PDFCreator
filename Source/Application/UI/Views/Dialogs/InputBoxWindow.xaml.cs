using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow(InputBoxWindowViewModel viewModel, ITranslator translator)
        {
            DataContext = viewModel;
            InitializeComponent();
            translator.Translate(this);
        }
    }
}