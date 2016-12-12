using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Queries;

namespace pdfforge.PDFCreator.Core.ComImplementation.Error
{
    public class ErrorNotifierCom : IErrorNotifier
    {
        public ActionResult Error { get; private set; }

        public void Notify(ActionResult actionResult)
        {
            Error = actionResult;
        }
    }
}
