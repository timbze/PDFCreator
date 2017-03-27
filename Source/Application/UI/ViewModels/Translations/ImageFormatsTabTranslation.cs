using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class ImageFormatsTabTranslation : ITranslatable
    {
        public string JpegColorsLabelContent { get; private set; } = "Colors:";
        public string JpegControlHeader { get; private set; } = "JPEG Settings";
        public string JpegQualityLabelContent { get; private set; } = "Quality (%):";
        public string JpegResolutionLabelContent { get; private set; } = "Resolution (DPI):";
        public string PngColorsLabelContent { get; private set; } = "Colors:";
        public string PngControlHeader { get; private set; } = "PNG Settings";
        public string PngResolutionLabelContent { get; private set; } = "Resolution (DPI):";
        public string TiffColorsLabelContent { get; private set; } = "Colors:";
        public string TiffControlHeader { get; private set; } = "TIFF Settings";
        public string TiffResolutionLabelContent { get; private set; } = "Resolution (DPI):";

        public EnumTranslation<JpegColor>[] JpegColorValues { get; set; } = EnumTranslation<JpegColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<PngColor>[] PngColorValues { get; set; } = EnumTranslation<PngColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<TiffColor>[] TiffColorValues { get; set; } = EnumTranslation<TiffColor>.CreateDefaultEnumTranslation();

    }
}
