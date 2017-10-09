using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpActionViewModel : HttpActionViewModel
    {
        public DesignTimeHttpActionViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator())
        {
        }
    }
}
