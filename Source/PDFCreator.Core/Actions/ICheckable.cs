using pdfforge.PDFCreator.Core.Settings;

namespace pdfforge.PDFCreator.Core.Actions
{
    interface ICheckable
    {
        ActionResult Check(ConversionProfile profile);
    }
}
