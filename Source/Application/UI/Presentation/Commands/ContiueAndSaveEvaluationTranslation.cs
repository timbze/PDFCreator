using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class ContiueAndSaveEvaluationTranslation : ITranslatable
    {
        public string Description { get; private set; } = "Do you want to continue and save your changes?";
        public string Title { get; private set; } = "Save Settings";
    }
}
