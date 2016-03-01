using pdfforge.PDFCreator.Core.Settings;

namespace pdfforge.PDFCreator.Helper
{
    internal static class PdfArchitectHelper
    {
        public static bool IsPdfArchitectInstalled
        {
            get { return PdfArchitectCheck.Installed(); }
        }

        public static string InstallPath
        {
            get { return PdfArchitectCheck.InstallationPath(); }
        }
    }
}
