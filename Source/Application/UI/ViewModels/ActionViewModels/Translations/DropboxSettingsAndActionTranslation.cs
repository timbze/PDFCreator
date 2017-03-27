using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class DropboxSettingsAndActionTranslation : ITranslatable
    {
        public string DisplayName { get; private set; } = "Upload to Dropbox";
        public string AddDropboxTokenLabelContent { get; private set; } = "Add Token:";
        public string CreateShareLinkText { get; private set; } = "Create a link to share the uploaded files. Anyone with the link can view this file.";
        public string DropboxAccountSelectContent { get; private set; } = "Please select Dropbox Account:";
        public string DropboxSharedFolderNameContent { get; private set; } = "Please define folder:";
        public string EnsureUniqueDropboxFilenamesText { get; private set; } = "Don't overwrite files (adds an increasing number in case of conflict).";
        public string SharedDropboxDirectoryPreviewContent { get; private set; } = "Preview:";
    }
}
