using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ApplicationSettings
{
    public partial class LicenseTab : UserControl
    {
        public LicenseTab()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as LicenseTabViewModel;
        }

        private void LicenseTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Display conversions from WPF bindings in current locale, i.e. Dates
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}