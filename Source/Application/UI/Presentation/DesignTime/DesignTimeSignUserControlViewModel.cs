using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignUserControlViewModel : SignUserControlViewModel
    {
        public DesignTimeSignUserControlViewModel() : base(null, null, null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator())
        {
        }
    }
}
