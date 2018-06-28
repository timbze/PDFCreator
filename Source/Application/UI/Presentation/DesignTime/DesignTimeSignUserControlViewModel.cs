using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignUserControlViewModel : SignUserControlViewModel
    {
        public DesignTimeSignUserControlViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator(), new SignaturePasswordCheck(), null, new DesignTimeTokenViewModelFactory())
        {
        }
    }
}
