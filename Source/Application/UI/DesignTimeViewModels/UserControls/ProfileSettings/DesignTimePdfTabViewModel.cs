using SystemWrapper.IO;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimePdfTabViewModel : PdfTabViewModel
    {
        public DesignTimePdfTabViewModel() : base(new PdfTabTranslation(), new DesignTimeInteractionInvoker(), new FileWrap(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()), new EditionHintOptionProvider(true), new DesignTimePDFProcessor(), new DesignTimeUserGuideHelper())
        {   }
    }
}