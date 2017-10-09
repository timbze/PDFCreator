using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.Pdf2Pdf;
using System;
using System.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class PdfAConverter
    {
        private readonly IPdfProcessor _pdfProcessor;

        public PdfAConverter(IPdfProcessor pdfProcessor)
        {
            _pdfProcessor = pdfProcessor;
        }

        public string ConvertToPdfA(string pdfFile, ConversionProfile profile)
        {
            if (!RequiresPdfAConversion(profile))
                return pdfFile;

            try
            {
                return DoConvertToPdfA(pdfFile, profile);
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " during PDF/A conversion:" + Environment.NewLine + ex.Message, ErrorCode.Conversion_PdfAError, ex);
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

        private string DoConvertToPdfA(string pdfFile, ConversionProfile profile)
        {
            var targetFile = Path.ChangeExtension(pdfFile, "_pdfa.pdf");

            var logfile = targetFile + ".log";
            using (var converter = new Pdf2Pdf())
            {
                converter.Compliance = DetermineCompliance(profile);
                converter.Convert(pdfFile, "", targetFile, logfile);
            }

            return targetFile;
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
