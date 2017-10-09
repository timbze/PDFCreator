using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpJobStepPasswordViewModel : HttpJobStepPasswordViewModel
    {
        public DesignTimeHttpJobStepPasswordViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()))
        {
            HttpAccountInfo = "Hier könnte Ihr Http-Account stehen.";
        }
    }
}
