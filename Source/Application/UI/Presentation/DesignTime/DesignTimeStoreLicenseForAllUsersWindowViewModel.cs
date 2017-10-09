using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeStoreLicenseForAllUsersWindowViewModel : StoreLicenseForAllUsersWindowViewModel
    {
        public DesignTimeStoreLicenseForAllUsersWindowViewModel()
            : base(new DesignTimeApplicationNameProvider(), new OsHelper(), null, null, new DesignTimeTranslationUpdater())
        { }
    }
}
