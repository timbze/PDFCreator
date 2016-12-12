using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeLicenseTabViewModel : LicenseTabViewModel
    {
        public DesignTimeLicenseTabViewModel()
            : base(null, new DesignTimeActivationHelper(), new TranslationProxy(), null, null)
        {
        }
    }
}