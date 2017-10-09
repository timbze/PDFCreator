using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.SettingsManagement;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public class PsDirectConversion : DirectConversionBase
    {
        public PsDirectConversion(ISettingsProvider settingsProvider, IJobInfoManager jobInfoManager, ISpoolerProvider spoolerProvider, IFile file, IDirectory directory, IPathSafe path) : base(settingsProvider, jobInfoManager, spoolerProvider)
        {
            File = file;
            Directory = directory;
            Path = path;
        }

        protected override IFile File { get; }
        protected override IDirectory Directory { get; }
        protected override IPathSafe Path { get; }

        protected override int GetNumberOfPages(string fileName)
        {
            var count = 0;
            try
            {
                using (var fs = File.OpenRead(fileName))
                using (var sr = new System.IO.StreamReader(fs.StreamInstance))
                {
                    while (sr.Peek() >= 0)
                    {
                        var readLine = sr.ReadLine();
                        if (readLine != null && readLine.Contains("%%Page:"))
                            count++;
                    }
                }
            }
            catch
            {
                Logger.Warn("Error while retrieving page count. Set value to 1.");
            }

            return count == 0 ? 1 : count;
        }

        protected override bool IsValid(string fileName)
        {
            return true;
        }
    }
}
