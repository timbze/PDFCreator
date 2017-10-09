using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public class BackgroundSettingsAndActionTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Add background (only for PDF)";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectBackgroundFile { get; private set; } = "Select background file";

        public string BackgroundFileLabelContent { get; private set; } = "Background File (PDF):";
        public string BackgroundRepetitionLabelContent { get; private set; } = "Repetition:";
        public string ShowBackgroundOnAttachmentText { get; private set; } = "Add background to attachment";
        public string ShowBackgroundOnCoverText { get; private set; } = "Add background to cover";
        public EnumTranslation<BackgroundRepetition>[] BackgroundRepetitionValues { get; private set; }
    }
}
