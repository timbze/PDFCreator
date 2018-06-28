using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class InteractiveProfileCheckerTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string InvalidSettings { get; private set; } = "Invalid settings";
        public string Error { get; private set; } = "Error";
    }
}
