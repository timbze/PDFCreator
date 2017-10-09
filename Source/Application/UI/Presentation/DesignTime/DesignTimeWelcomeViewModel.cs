using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWelcomeViewModel : WelcomeViewModel
    {
        public DesignTimeWelcomeViewModel()
            : base(new ProcessStarter(), new ButtonDisplayOptions(false, false), new DesignTimeUserGuideHelper(), new DesignTimeTranslationUpdater())
        {
        }
    }
}
