using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeLicenseWindowViewModel : LicenseWindowViewModel
    {
        public DesignTimeLicenseWindowViewModel() : base(new DesignTimeLicenseTabViewModel(),  new ApplicationNameProvider("PDFCreator"), new VersionHelper(new AssemblyHelper()), new DesignTimeUserGuideHelper())
        {
        }
    }
}