using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Interface
{
    public interface IDefaultViewerAction : IAction
    {
        ActionResult OpenWithArchitect(string filePath);

        ActionResult OpenOutputFile(string filePath);
    }
}
