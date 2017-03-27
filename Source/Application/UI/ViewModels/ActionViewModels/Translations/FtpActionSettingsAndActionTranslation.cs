using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class FtpActionSettingsAndActionTranslation : ITranslatable
    {
        public string Description { get; private set; } = "The FTP action allows to upload the created documents to a server via FTP.";
        public string DisplayName { get; private set; } = "Upload with FTP";
        public string PasswordDescription { get; private set; } = "FTP server password:";
        public string PasswordTitle { get; private set; } = "FTP server password";
        public string DirectoryAddTokenText { get; private set; } = "Add Token:";
        public string DirectoryPreviewText { get; private set; } = "Preview:";
        public string DirectoryText { get; private set; } = "Directory:";
        public string EnsureUniqueFilenamesText { get; private set; } = "Don't overwrite files (adds an increasing number in case of conflict)";
        public string ServerText { get; private set; } = "Server:";
        public string SetPasswordText { get; private set; } = "Set Password";
        public string UsernameText { get; private set; } = "User Name:";
    }
}
