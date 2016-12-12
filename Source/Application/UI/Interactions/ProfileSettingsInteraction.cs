using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class ProfileSettingsInteraction : IInteraction
    {
        public ProfileSettingsInteraction(PdfCreatorSettings settings, IGpoSettings gpoSettings)
        {
            UnchangedOriginalSettings = settings;
            Settings = settings.Copy();
            GpoSettings = gpoSettings;
        }

        public PdfCreatorSettings UnchangedOriginalSettings { get; }

        public PdfCreatorSettings Settings { get; set; }
        public IGpoSettings GpoSettings { get; set; }

        public bool ApplySettings { get; set; }
    }
}