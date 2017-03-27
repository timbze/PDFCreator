using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeStoreLicenseForAllUsersWindowViewModel : StoreLicenseForAllUsersWindowViewModel
    {
        public DesignTimeStoreLicenseForAllUsersWindowViewModel() 
            : base(new ApplicationNameProvider("PDFCreator"), new OsHelper(), null, null, new StoreLicenseForAllUsersWindowTranslation())
        {   }
    }
}
