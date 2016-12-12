using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class ApplicationSettingsInteraction : IInteraction
    {
        public ApplicationSettingsInteraction(PdfCreatorSettings settings, IGpoSettings gpoSettings)
        {
            Settings = settings;
            GpoSettings = gpoSettings;
        }

        public PdfCreatorSettings Settings { get; set; }
        public IGpoSettings GpoSettings { get; set; }

        public bool Success { get; set; }
    }
}