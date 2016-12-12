using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    internal class DesignTimeSignaturePasswordCheck : ISignaturePasswordCheck
    {
        public bool IsValidPassword(string certificateFile, string password)
        {
            return false;
        }
    }
}