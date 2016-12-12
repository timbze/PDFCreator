using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class UpdateDownloadWindow : Window
    {
        public UpdateDownloadWindow(UpdateDownloadWindowViewModel viewModel, ITranslator translator)
        {
            InitializeComponent();

            DataContext = viewModel;
            translator.Translate(this);
        }
    }
}