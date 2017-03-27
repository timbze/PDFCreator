using System;
using System.IO;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.Pdf2Pdf;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class PdfAConverter
    {
        private readonly IPdfProcessor _pdfProcessor;

        public PdfAConverter(IPdfProcessor pdfProcessor)
        {
            _pdfProcessor = pdfProcessor;
        }

        public void ConvertToPdfA(string pdfFile, ConversionProfile profile)
        {
            if (!RequiresPdfAConversion(profile))
                return;

            var tmpFile = _pdfProcessor.MoveFileToPreProcessFile(pdfFile, "PrePdfA");

            try
            {
                DoConvertToPdfA(tmpFile, pdfFile, profile);
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " during PDF/A conversion:" + Environment.NewLine + ex.Message, ErrorCode.Conversion_PdfAError);
            }
            finally
            {
                //delete copy of original file
                if (!string.IsNullOrEmpty(tmpFile))
                    if (File.Exists(tmpFile))
                        File.Delete(tmpFile);
            }
        }

        private bool RequiresPdfAConversion(ConversionProfile profile)
        {
            switch (profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                    return true;
                default:
                    return false;
            }
        }

        private void DoConvertToPdfA(string tmpFile, string targetFile, ConversionProfile profile)
        {
            var logfile = targetFile + ".log";
            using (var converter = new Pdf2Pdf())
            {
                converter.Compliance = DetermineCompliance(profile);
                converter.Convert(tmpFile, "", targetFile, logfile);                
            }
        }

        private PDFCompliance DetermineCompliance(ConversionProfile profile)
        {
            switch (profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                    return PDFCompliance.ePDFA1b;
                case OutputFormat.PdfA2B:
                    return PDFCompliance.ePDFA2b;
                default:
                    return PDFCompliance.ePDFUnk;
            }
        }
    }
}
