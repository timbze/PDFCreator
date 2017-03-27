using System.Text;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ProfileSettings
{
    public partial class DocumentTab : UserControl
    {
        public DocumentTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as DocumentTabViewModel;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var caret = StampText.CaretIndex; 
            var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(StampText.Text);
            StampText.Text = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            StampText.CaretIndex = caret;
        }
    }
}