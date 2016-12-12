using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeOfflineActivationWindow : OfflineActivationViewModel
    {
        public DesignTimeOfflineActivationWindow()
            : base(new ProcessStarter(), new DesignTimeUserGuideHelper(), new DesignTimeActivationHelper(), new TranslationProxy())
        {
            var interaction = new OfflineActivationInteraction("SomeLicenseKey");
            interaction.LicenseServerAnswer = "SomeLSA";
            SetInteraction(interaction);
        }
    }
}