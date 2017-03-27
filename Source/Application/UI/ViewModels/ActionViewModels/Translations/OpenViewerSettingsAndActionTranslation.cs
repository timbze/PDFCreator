using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class OpenViewerSettingsAndActionTranslation : ITranslatable
    {
        public string ArchitectNotInstalled { get; private set; } = "PDF Architect is not installed.\r\nDo You want to download it from pdfforge.org?";
        public string Description { get; private set; } = "Open document after saving.";
        public string DisplayName { get; private set; } = "Open document";
        public string GetPdfArchitectButtonContent { get; private set; } = "More on PDF Architect";
        public string OpenWithArchitectCheckBoxContent { get; private set; } = "Open PDF files with PDF Architect";
        public string PdfArchitectAddModulesText { get; private set; } = "Obtain powerful modules to do more like";
        public string PdfArchitectConvertBackText { get; private set; } = "Convert PDFs to Word, Excel and more";
        public string PdfArchitectEditText { get; private set; } = "Edit PDFs like with a word processor";
        public string PdfArchitectFreeText { get; private set; } = "FREE features include:";
        public string PdfArchitectIntroText { get; private set; } = "Our full-featured PDF Editor";
        public string PdfArchitectOcrText { get; private set; } = "Text recognition (OCR)";
        public string PdfArchitectSplitAndMergeText { get; private set; } = "Split and merge PDFs";
        public string PdfArchitectViewAndPrintText { get; private set; } = "View and print PDFs";
    }
}
