using System;
using System.IO;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public class PdfDirectConversion : DirectConversionBase
    {
        private readonly IPdfProcessor _pdfProcessor;

        public PdfDirectConversion(IPdfProcessor pdfProcessor, ISettingsProvider settingsProvider, IJobInfoManager jobInfoManager) : base (settingsProvider, jobInfoManager)
        {
            _pdfProcessor = pdfProcessor;
        }

        internal override int GetNumberOfPages(string fileName)
        {
            return _pdfProcessor.GetNumberOfPages(fileName);
        }

        internal override bool IsValid(string fileName)
        {
            string firstChars;
            using (var reader = new StreamReader(fileName))
            {
                var charArray = new char[4];
                reader.Read(charArray, 0, 4);
                firstChars = new string(charArray);
            }
            if (firstChars.StartsWith("%PDF", StringComparison.InvariantCultureIgnoreCase))
                return true;

            Logger.Error(fileName + " is not a valid PDF file");
            return false;
        }
    }
}