using SystemWrapper.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ProfileSettings
{
    public class DesignTimePdfTabViewModel : PdfTabViewModel
    {
        public DesignTimePdfTabViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker(), new FileWrap(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }
}