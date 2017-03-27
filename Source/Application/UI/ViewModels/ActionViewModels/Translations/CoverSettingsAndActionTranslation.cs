using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class CoverSettingsTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string Description { get; private set; } = "Add a cover to your documents.\r\nThe cover file must be a PDF file and may contain multiple pages.";
        public string DisplayName { get; private set; } = "Add cover";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectCoverFile { get; private set; } = "Select cover file";
        public string CoverFileText { get; private set; } = "Cover file (PDF):";

    }
}
