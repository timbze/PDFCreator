using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeWelcomeWindowViewModel : WelcomeWindowViewModel
    {
        public DesignTimeWelcomeWindowViewModel()
            : base(new ProcessStarter(), new ButtonDisplayOptions(false, false), new DesignTimeUserGuideHelper(), new WelcomeWindowTranslation())
        {
        }
    }
}