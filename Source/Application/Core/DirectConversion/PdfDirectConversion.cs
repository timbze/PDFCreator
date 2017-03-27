using System;
using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public class PdfDirectConversion : DirectConversionBase
    {
        private readonly IPdfProcessor _pdfProcessor;

        public PdfDirectConversion(IPdfProcessor pdfProcessor, ISettingsProvider settingsProvider, IJobInfoManager jobInfoManager, ISpoolerProvider spoolerProvider, IFile file, IDirectory directory, IPathSafe path) : base(settingsProvider, jobInfoManager, spoolerProvider)
        {
            _pdfProcessor = pdfProcessor;
            File = file;
            Directory = directory;
            Path = path;
        }

        protected override IFile File { get; }
        protected override IDirectory Directory { get; }
        protected override IPathSafe Path { get; }

        protected override int GetNumberOfPages(string fileName)
        {
            return _pdfProcessor.GetNumberOfPages(fileName);
        }

        protected override bool IsValid(string fileName)
        {
            string firstChars = "";
            using (var fs = File.OpenRead(fileName))
            using (var sr = new System.IO.StreamReader(fs.StreamInstance))
            {
                if (fs.Length >= 4)
                {
                    var charArray = new char[4];
                    sr.Read(charArray, 0, 4);
                    firstChars = new string(charArray);
                }
            }
            if (firstChars.StartsWith("%PDF", StringComparison.InvariantCultureIgnoreCase))
                return true;

            Logger.Error(fileName + " is not a valid PDF file");
            return false;
        }
    }
}