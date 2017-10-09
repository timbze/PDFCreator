using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script
{
    public class ScriptTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Run script";
        public string ExecutableFiles { get; private set; } = "Executable files";
        public string SelectScriptTitle { get; private set; } = "Select script file";
        public string AdditionalParametersText { get; private set; } = "Additional Script Parameters:";
        public string ScriptCallPreviewText { get; private set; } = "Script Call Preview:";
        public string ScriptFileText { get; private set; } = "Script File:";
        public string WaitForScriptText { get; private set; } = "Wait until the script execution has ended";
    }
}
