using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class SaveTabTranslation : ITranslatable
    {
        public string InteractiveToggle { get; private set; } = "Interactive";
        public string InteractiveHint { get; private set; } = "Shows a window to select the filename, output format etc.";
        public string AutosaveToggle { get; private set; } = "Automatic";
        public string AutosaveHint { get; private set; } = "Converts all documents without asking for settings. This requires more settings to be defined in the profile.";

        public string SaveTabText { get; private set; } = "Save";
        public string SaveTitle { get; private set; } = "Save";
        public string SelectFilename { get; private set; } = "Filename:";
        public string SelectFolder { get; private set; } = "Folder:";

        public string SkipPrintDialog { get; private set; } = "Skip print dialog (start with save dialog)";
        public string OpenFileAfterSaving { get; private set; } = "Open file after saving";
        public string OpenWithPdfArchitect { get; private set; } = "Use PDF Architect for PDF files";
        public string ChooseFolder { get; private set; } = "Choose a folder";

        public string DontOverwriteFiles { get; private set; } = "Don't overwrite existing files";
        public string UniqueFilenameHint { get; private set; } = "If a file already exists, an incrementing number is added to the filename, i.e. 'file_5.pdf'";

        public string SkipPrintDialogHint { get; private set; } = "To be able to skip the print dialog and directly proceed to the save file dialog, you need PDFCreator Plus, PDFCreator Business or PDFCreator Terminal Server";

        public string ShowWindowAfterConversion { get; private set; } = "Show quick actions after converting a document";
    }
}
