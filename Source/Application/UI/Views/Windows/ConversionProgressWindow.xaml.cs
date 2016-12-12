using System.Windows;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class ConversionProgressWindow : Window
    {
        public ConversionProgressWindow()
        {
            InitializeComponent();
            // ViewModel is set in Xaml because it has no dependencies
        }
    }
}