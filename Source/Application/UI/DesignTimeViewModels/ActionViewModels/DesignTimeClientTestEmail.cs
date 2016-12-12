using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.ActionViewModels
{
    public class DesignTimeClientTestEmail : IClientTestEmail
    {
        public bool SendTestEmail(EmailClientSettings clientSettings)
        {
            return true;
        }
    }
}