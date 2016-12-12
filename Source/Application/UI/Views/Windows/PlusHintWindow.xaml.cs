using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class PlusHintWindow : Window
    {
        public PlusHintWindow(ITranslator translator, PlusHintWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            translator.Translate(this);
        }
    }
}