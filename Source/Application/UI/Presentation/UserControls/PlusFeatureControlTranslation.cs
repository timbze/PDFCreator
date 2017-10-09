using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class PlusFeatureControlTranslation : ITranslatable
    {
        public string PlusFeature { get; private set; } = "PLUS FEATURE";
        public string BusinessFeature { get; private set; } = "BUSINESS FEATURE";

        public string PlusRequiredHint { get; private set; } = "You need PDFCreator Plus, PDFCreator Business or PDFCreator Terminal Server for this feature. Click for more information.";
        public string BusinessRequiredHint { get; private set; } = "You need PDFCreator Business or PDFCreator Terminal Server for this feature. Click for more information.";

        public string PlusHint { get; private set; } = "Unlock the premium features with PDFCreator Plus!";
    }
}
