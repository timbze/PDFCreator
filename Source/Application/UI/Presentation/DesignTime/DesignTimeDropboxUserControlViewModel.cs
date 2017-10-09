using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDropboxUserControlViewModel : DropboxUserControlViewModel
    {
        public DesignTimeDropboxUserControlViewModel()
            : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator())
        {
        }
    }
}
