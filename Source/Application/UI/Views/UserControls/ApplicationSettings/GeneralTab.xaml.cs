using System;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ApplicationSettings
{
    public partial class GeneralTab : UserControl
    {
        public GeneralTab()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        public Action PreviewLanguageAction { private get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as GeneralTabViewModel;

            if (viewModel == null)
                return;

            viewModel.PreviewTranslation += (o, args) => PreviewLanguageAction?.Invoke();
        }
    }
}