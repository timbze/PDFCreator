using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class EvaluateSettingsAndNotifyUserTranslation : ITranslatable
    {
        public string UnsavedChanges { get; private set; } = "Do you want to save your changes?";
        public string PDFCreatorSettings { get; private set; } = "PDFCreator Settings";
        public string DefaultViewer { get; private set; } = "Default Viewer";
        public string InvalidSettings { get; private set; } = "You have invalid settings.";
        public string SaveAnyway { get; private set; } = "Do you want to save anyway?";
        public string ProceedAnyway { get; private set; } = "Do you want to proceed anyway?";
    }
}
