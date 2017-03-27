using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
    public class ApplicationSettingsWindowTranslation : ITranslatable
    {
        public string AdminPrivilegesRequired { get; private set; } = "Admin privileges required";
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string DebugTabText { get; private set; } = "Debug";
        public string Error { get; private set; } = "Error";
        public string GeneralTabText { get; private set; } = "General";
        public string HelpButtonContent { get; private set; } = "Help";
        public string IniFileFilter { get; private set; } = "INI files (*.ini)|*.ini";
        public string InvalidSettings { get; private set; } = "Invalid settings";
        public string InvalidSettingsWarning { get; private set; } = "The file does not appear to contain valid PDFCreator settings.";
        public string LicenseTabText { get; private set; } = "License";
        public string LoadSettingsFromFileWarning { get; private set; } = "By loading the file all settings and profiles will be overwritten. Do you want to continue?";
        public string OperationRequiresAdminPrivileges { get; private set; } = "This operation requires admin privileges and it looks like you are not an admin. Do you want to continue anyway?\r\nNote: It is safe to continue even if you are unsure if you have appropriate rights, but the operation will not be completed.";
        public string OverwriteAllSettings { get; private set; } = "Overwrite all settings";
        public string PrintersTabText { get; private set; } = "Printers";
        public string SaveButtonContent { get; private set; } = "Save";
        public string SufficientPermissions { get; private set; } = "Operation failed. You probably do not have sufficient permissions.";
        public string Title { get; private set; } = "PDFCreator Settings";
        public string TitleTabText { get; private set; } = "Title";
        public string TitleText { get; private set; } = "Application Settings";
        private string SetupFileMissing { get; set; } = "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!";

        public string GetFormattedSetupFileMissing(string fileName)
        {
            return string.Format(SetupFileMissing, fileName);
        }


    }
}
