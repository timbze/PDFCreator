using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum PngColor
    {
        [Translation("16 Million Colors with Transparency (32 Bit)")]
        Color32BitTransp,
        [Translation("16 Million Colors (24 Bit)")]
        Color24Bit,
        [Translation("256 Colors (8 Bit)")]
        Color8Bit,
        [Translation("16 Color (4 Bit)")]
        Color4Bit,
        [Translation("Grayscale (8 Bit)")]
        Gray8Bit,
        [Translation("Black & White (2 Bit)")]
        BlackWhite //2Bit
    }
}