using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.Views.WindowsApi;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class PrintJobWindow : Window
    {
        private readonly ViewCustomization _customization;

        public PrintJobWindow(PrintJobViewModel viewModel, ViewCustomization customization)
        {
            _customization = customization;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_customization.ApplyCustomization)
                Title = _customization.PrintJobWindowCaption;

            FlashWindow.Flash(this, 3);
        }
    }
}