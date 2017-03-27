using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeOfflineActivationWindow : OfflineActivationViewModel
    {
        public DesignTimeOfflineActivationWindow()
            : base(new ProcessStarter(), new DesignTimeUserGuideHelper(), new DesignTimeOfflineActivator(), new OfflineActivationViewModelTranslation())
        {
            var interaction = new OfflineActivationInteraction("SomeLicenseKey");
            interaction.LicenseServerAnswer = "SomeLSA";
            SetInteraction(interaction);
        }
    }
}