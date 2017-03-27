using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeUpdateAvailableViewModel : UpdateAvailableViewModel
    {
        public DesignTimeUpdateAvailableViewModel() : base(new UpdateManagerTranslation(), null, new ApplicationNameProvider("PDFCreator"))
        {
        }
    }
}
