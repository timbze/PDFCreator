using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.ActionViewModels
{
    public class DesignTimeArchitectCheck : IPdfArchitectCheck
    {
        public string GetInstallationPath()
        {
            return "";
        }

        public bool IsInstalled()
        {
            return true;
        }
    }
}