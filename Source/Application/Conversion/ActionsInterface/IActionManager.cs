using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IActionManager
    {
        IEnumerable<IAction> GetAllApplicableActions(Job job);
    }
}
