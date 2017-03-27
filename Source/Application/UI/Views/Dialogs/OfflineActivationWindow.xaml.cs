using System.Windows;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    /// <summary>
    ///     Interaction logic for OfflineActivationWindow.xaml
    /// </summary>
    public partial class OfflineActivationWindow : Window
    {
        public OfflineActivationWindow(OfflineActivationViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}