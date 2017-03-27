using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum TiffColor
    {
        [Translation("16 Million Colors (24 Bit)")]
        Color24Bit,
        [Translation("4096 Colors (12 Bit)")]
        Color12Bit,
        [Translation("Grayscale (8 Bit)")]
        Gray8Bit,
        [Translation("Black & White (2 Bit G3 Fax)")]
        BlackWhiteG3Fax,
        [Translation("Black & White (2 Bit G4 Fax)")]
        BlackWhiteG4Fax,
        [Translation("Black & White (2 Bit LZW)")]
        BlackWhiteLzw
    }
 }
