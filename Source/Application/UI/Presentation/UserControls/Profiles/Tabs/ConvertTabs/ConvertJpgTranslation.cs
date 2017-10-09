using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertJpgTranslation : ITranslatable
    {
        public string JpegColorsLabelContent { get; private set; } = "Colors:";
        public string JpegControlHeader { get; private set; } = "JPEG Settings";
        public string JpegQualityLabelContent { get; private set; } = "Quality (%):";
        public string JpegResolutionLabelContent { get; private set; } = "Resolution (DPI):";

        public EnumTranslation<JpegColor>[] JpegColorValues { get; set; } = EnumTranslation<JpegColor>.CreateDefaultEnumTranslation();
    }
}
