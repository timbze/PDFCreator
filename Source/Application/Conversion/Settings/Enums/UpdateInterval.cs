using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum UpdateInterval
    {

        [Translation("Never")]
        Never,

        [Translation("Daily")]
        Daily,

        [Translation("Weekly")]
        Weekly,

        [Translation("Monthly")]
        Monthly
    }
}