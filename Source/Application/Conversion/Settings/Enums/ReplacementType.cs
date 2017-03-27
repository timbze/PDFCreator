using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum ReplacementType
    {
        [Translation("Replace")]
        Replace,

        [Translation("Start")]
        Start,

        [Translation("End")]
        End,

        [Translation("RegEx")]
        RegEx
    }
}