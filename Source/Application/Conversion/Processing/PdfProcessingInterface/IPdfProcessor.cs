using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IPdfProcessor
    {
        void Init(Job job);
        bool ProcessingRequired(ConversionProfile profile);
        void ProcessPdf(Job job);
        int GetNumberOfPages(string pdfFile);
        string DeterminePdfVersion(ConversionProfile profile);
        string MoveFileToPreProcessFile(string pdfFile, string appendix);
    }
}