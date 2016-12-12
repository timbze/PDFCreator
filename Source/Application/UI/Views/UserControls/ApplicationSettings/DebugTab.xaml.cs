using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ApplicationSettings
{
    public partial class DebugTab : UserControl
    {
        public DebugTab()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as DebugTabViewModel;
            viewModel?.Translator.Translate(this);
        }
    }
}