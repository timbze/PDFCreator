using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface ICheckable
    {
        ActionResult Check(ConversionProfile profile, Accounts accounts);
    }
}
