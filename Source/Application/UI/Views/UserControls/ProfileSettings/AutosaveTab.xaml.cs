using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ProfileSettings
{
    public partial class AutosaveTab : UserControl
    {
        public AutosaveTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as AutoSaveTabViewModel;
            viewModel?.Translator.Translate(this);
        }
    }
}