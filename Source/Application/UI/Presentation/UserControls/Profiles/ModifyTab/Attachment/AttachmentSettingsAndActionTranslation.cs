using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment
{
    public class AttachmentSettingsAndActionTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Add attachment";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectAttachmentFile { get; private set; } = "Select attachment file";
        public string AttachmentFileText { get; private set; } = "Attachment file (PDF):";
    }
}
