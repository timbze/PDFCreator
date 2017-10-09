using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp
{
    public class DocumentTabTranslation : ITranslatable
    {
        public string StampControlHeader { get; private set; } = "Stamp";
        public string StampFontAsOutlineCheckBoxContent { get; private set; } = "Show Font as outline";
        public string StampFontColorLabelContent { get; private set; } = "Font Color:";
        public string StampFontLabelContent { get; private set; } = "Font:";
        public string StampOutlineWidthLabelContent { get; private set; } = "Outline Width:";
        public string StampTextLabelContent { get; private set; } = "Stamp Text:";
        public string FontFileNotSupported { get; private set; } = "The selected font is not supported. Please select a different font.";
    }
}
