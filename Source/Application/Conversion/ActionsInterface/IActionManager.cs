using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IActionManager
    {
        IEnumerable<IAction> GetAllApplicableActions(Job job);
    }
}