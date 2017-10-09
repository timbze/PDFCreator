using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertPngTranslation : ITranslatable
    {
        public string PngColorsLabelContent { get; private set; } = "Colors:";
        public string PngControlHeader { get; private set; } = "PNG Settings";
        public string PngResolutionLabelContent { get; private set; } = "Resolution (DPI):";

        public EnumTranslation<PngColor>[] PngColorValues { get; set; } = EnumTranslation<PngColor>.CreateDefaultEnumTranslation();
    }
}
