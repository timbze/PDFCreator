using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum SignaturePage
    {
        [Translation("First Page")]
        FirstPage,

        [Translation("Last Page")]
        LastPage,

        [Translation("Custom Page")]
        CustomPage
    }
}