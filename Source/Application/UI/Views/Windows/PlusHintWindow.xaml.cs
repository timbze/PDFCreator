using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class PlusHintWindow : Window
    {
        public PlusHintWindow(PlusHintWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}