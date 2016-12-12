using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeUpdateAvailableViewModel : UpdateAvailableViewModel
    {
        public DesignTimeUpdateAvailableViewModel() : base(new TranslationProxy(), null, new ApplicationNameProvider("PDFCreator"))
        {
        }
    }
}
