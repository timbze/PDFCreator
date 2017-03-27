using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeLicenseTabViewModel : LicenseTabViewModel
    {
        public DesignTimeLicenseTabViewModel()
            : base(null, new DesignTimeLicenseChecker(), new DesignTimeOfflineActivator(), null, null, new LicenseTabTranslation())
        {   }

    }
}