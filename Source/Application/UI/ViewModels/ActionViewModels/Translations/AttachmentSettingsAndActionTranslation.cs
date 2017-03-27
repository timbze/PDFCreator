using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class AttachmentSettingsAndActionTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string Description { get; private set; } = "Add an attachment to your documents.\r\nThe attachment file must be a PDF file and may contain multiple pages.";
        public string DisplayName { get; private set; } = "Add attachment";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectAttachmentFile { get; private set; } = "Select attachment file";
        public string AttachmentFileText { get; private set; } = "Attachment file (PDF):";

    }
}
