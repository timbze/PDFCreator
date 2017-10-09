using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl, IInteraction
    {
        public WelcomeView()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<WelcomeViewModel>();

            InitializeComponent();
        }
    }
}
