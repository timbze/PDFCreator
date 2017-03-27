using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum SelectPrinter
    {
        [Translation("default printer")]
        DefaultPrinter,
        [Translation("printer dialog")]
        ShowDialog,
        [Translation("select printer ->")]
        SelectedPrinter
    }
}