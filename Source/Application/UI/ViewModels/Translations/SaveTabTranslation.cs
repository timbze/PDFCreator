using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class SaveTabTranslation : ITranslatable
    {
        public string ConversionControlHeader { get; private set; } = "Conversion";
        public string DefaultFileFormatLabelContent { get; private set; } = "Select the default file format:";
        public string FilenameControlHeader { get; private set; } = "Filename";
        public string FilenamePreviewLabelContent { get; private set; } = "Preview:";
        public string FilenameTemplateLabelContent { get; private set; } = "Filename Template:";
        public string FilenameTokenLabelContent { get; private set; } = "Add Token:";
        public string SaveDialogFolderCheckBoxContent { get; private set; } = "Set default output folder (else the last saved file location will be opened)";
        public string SaveDialogFolderControlHeader { get; private set; } = "Save Dialog Folder";
        public string SaveDialogFolderPreviewLabelContent { get; private set; } = "Preview:";
        public string SaveDialogFolderTemplateLabelContent { get; private set; } = "Folder Template:";
        public string SaveDialogFolderTokenLabelContent { get; private set; } = "Add Token:";
        public string ShowProgressCheckBoxContent { get; private set; } = "Show progress during conversion";
        public string SkipPrintDialogCheckBoxContent { get; private set; } = "Skip print dialog (directly proceed to the save dialog)";
        public string SelectSaveDialogFolder { get; private set; } = "Select save dialog folder";

    }
}
