using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertTiffTranslation : ITranslatable
    {
        public string TiffColorsLabelContent { get; private set; } = "Colors:";
        public string TiffControlHeader { get; private set; } = "TIFF Settings";
        public string TiffResolutionLabelContent { get; private set; } = "Resolution (DPI):";

        public EnumTranslation<TiffColor>[] TiffColorValues { get; set; } = EnumTranslation<TiffColor>.CreateDefaultEnumTranslation();
    }
}
