using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class AutosaveTabTranslation : ITranslatable
    {
        public string AutomaticSavingCheckBoxContent { get; private set; } = "Enable automatic saving";
        public string AutomaticSavingControlHeader { get; private set; } = "Automatic Saving";
        public string AutomaticSavingHintText { get; private set; } = "Edit profiles without print dialog by opening PDFCreator in Windows Explorer";
        public string EnsureUniqueFilenamesCheckBoxContent { get; private set; } = "Ensure unique filenames (Do not overwrite existing files)";
        public string TargetFolderLabelContent { get; private set; } = "Target Folder:";
        public string TargetFolderPreviewLabelContent { get; private set; } = "Preview:";
        public string TargetFolderTokenLabelContent { get; private set; } = "Add Token:";
        public string SelectAutoSaveFolder { get; private set; } = "Select folder for automatic saving";


    }
}
