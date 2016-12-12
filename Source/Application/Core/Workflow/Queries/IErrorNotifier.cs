using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IErrorNotifier
    {
        void Notify(ActionResult actionResult);
    }
}