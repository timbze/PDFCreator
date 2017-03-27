using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ProfileSettings
{
    public partial class ActionsTab : UserControl
    {
        public ActionsTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public ActionsTabViewModel ViewModel { get; private set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var dataContext = DataContext as ActionsTabViewModel;
            if (dataContext == null)
                return;

            ViewModel = dataContext;
        }
    }
}