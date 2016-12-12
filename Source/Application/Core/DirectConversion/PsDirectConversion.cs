using System.IO;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public class PsDirectConversion : DirectConversionBase
    {
        public PsDirectConversion(ISettingsProvider settingsProvider, IJobInfoManager jobInfoManager) : base(settingsProvider, jobInfoManager)
        {
        }

        internal override int GetNumberOfPages(string fileName)
        {
            var count = 0;
            try
            {
                using (var fs = File.OpenRead(fileName))
                {
                    var sr = new StreamReader(fs);

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

        internal override bool IsValid(string fileName)
        {
            return true;
        }
    }
}