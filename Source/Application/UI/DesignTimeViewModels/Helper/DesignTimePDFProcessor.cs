using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimePDFProcessor : IPdfProcessor
    {
        public void Init(Job job)
        {
            
        }

        public bool ProcessingRequired(ConversionProfile profile)
        {
            return false;
        }

        public void ProcessPdf(Job job)
        {
            
        }

        public int GetNumberOfPages(string pdfFile)
        {
            return 1;
        }

        public string DeterminePdfVersion(ConversionProfile profile)
        {
            return "1.4";
        }

        public string MoveFileToPreProcessFile(string pdfFile, string appendix)
        {
            return "";
        }

        public bool IsLicenseValid()
        {
            return true;
        }
    }
}
