using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ApplicationSettings
{
    public partial class PrinterTab : UserControl
    {
        public PrinterTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as PrinterTabViewModel;

            viewModel?.Translator.Translate(this);
        }
    }
}