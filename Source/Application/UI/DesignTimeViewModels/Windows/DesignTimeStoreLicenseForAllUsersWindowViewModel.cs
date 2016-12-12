using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeStoreLicenseForAllUsersWindowViewModel : StoreLicenseForAllUsersWindowViewModel
    {
        public DesignTimeStoreLicenseForAllUsersWindowViewModel() 
            : base(new ApplicationNameProvider("PDFCreator"), new OsHelper(), null, null, new TranslationProxy())
        {   }
    }
}
