using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ProfileSettings
{
    public partial class ImageFormatsTab : UserControl
    {
        public ImageFormatsTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as ImageFormatsTabViewModel;
            viewModel?.Translator.Translate(this);
        }
    }
}