using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeAboutWindowViewModel : AboutWindowViewModel
    {
        public DesignTimeAboutWindowViewModel() :
            base(new ProcessStarter(), new ApplicationNameProvider("PDFCreator"), new VersionHelper(new AssemblyHelper()), new DesignTimeUserGuideHelper(), new ButtonDisplayOptions(false, false), new AboutWindowTranslation())
        {
        }
    }
}