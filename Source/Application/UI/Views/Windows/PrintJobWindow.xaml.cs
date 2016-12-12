using System;
using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.Views.WindowsApi;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class PrintJobWindow : Window
    {
        private readonly ITranslator _translator;
        private readonly ViewCustomization _customization;

        public PrintJobWindow(PrintJobViewModel viewModel, ITranslator translator, ViewCustomization customization)
        {
            _translator = translator;
            _customization = customization;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _translator.Translate(this);

            if (_customization.ApplyCustomization)
                Title = _customization.PrintJobWindowCaption;

            FlashWindow.Flash(this, 3);
        }
    }
}