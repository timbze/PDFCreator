using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller.Routing;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface IShellManager
    {
        void ShowMainShell(StartupRoutine startupRoutine);

        void ShowPrintJobShell(Job job);

        void MainShellToFront();
    }
}
