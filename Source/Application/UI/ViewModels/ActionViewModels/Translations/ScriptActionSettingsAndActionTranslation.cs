using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class ScriptActionSettingsAndActionTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string Description { get; private set; } = "The script action runs a custom script or application after the conversion. This allows you to further process the output.";
        public string DisplayName { get; private set; } = "Run script";
        public string ExecutableFiles { get; private set; } = "Executable files";
        public string SelectScriptTitle { get; private set; } = "Select script file";
        public string AdditionalParametersText { get; private set; } = "Additional Script Parameters:";
        public string AddParameterTokenText { get; private set; } = "Add Token:";
        public string AddScriptTokenText { get; private set; } = "Add Token:";
        public string ScriptCallPreviewText { get; private set; } = "Script Call Preview:";
        public string ScriptFileText { get; private set; } = "Script File:";
        public string WaitForScriptText { get; private set; } = "Wait until the script execution has ended";
    }
}
