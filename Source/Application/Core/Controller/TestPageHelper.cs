using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface ITestPageHelper
    {
        void CreateTestPage();
    }

    public class TestPageHelper : ITestPageHelper
    {
        private readonly IVersionHelper _versionHelper;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IOsHelper _osHelper;
        private readonly string _spoolFolder;

        public TestPageHelper(IVersionHelper versionHelper, IOsHelper osHelper, ISpoolerProvider spoolerProvider,
            IJobInfoQueue jobInfoQueue, IJobInfoManager jobInfoManager, ApplicationNameProvider applicationNameProvider)
        {
            _versionHelper = versionHelper;
            _osHelper = osHelper;
            _jobInfoQueue = jobInfoQueue;
            _jobInfoManager = jobInfoManager;
            _applicationNameProvider = applicationNameProvider;
            _spoolFolder = spoolerProvider.SpoolFolder;
        }

        /// <summary>
        ///     Creates a testpage in the spool folder and adds it to the JobInfoQueue
        /// </summary>
        public void CreateTestPage()
        {
            var tempPath = Path.Combine(_spoolFolder, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            var psFileContent = GetPsFileContent();
            var psFilePath = Path.Combine(tempPath, "testpage.ps");
            File.WriteAllText(psFilePath, psFileContent);

            var infFileContent = GetInfFileContent();
            var infFilePath = Path.Combine(tempPath, "testpage.inf");
            File.WriteAllText(infFilePath, infFileContent, Encoding.Unicode);

            var testPageJob = _jobInfoManager.ReadFromInfFile(infFilePath);

            _jobInfoQueue.Add(testPageJob);
        }

        private string GetInfFileContent()
        {
            var sb = new StringBuilder();

            sb.AppendLine("[0]");
            sb.AppendLine("SessionId=" + Process.GetCurrentProcess().SessionId);
            sb.AppendLine("WinStation=Console");
            sb.AppendLine("UserName=" + Environment.UserName);
            sb.AppendLine("ClientComputer=" + Environment.MachineName);
            sb.AppendLine("SpoolFileName=testpage.ps");
            sb.AppendLine("PrinterName=PDFCreator");
            sb.AppendLine("JobId=1");
            sb.AppendLine("TotalPages=1");
            sb.AppendLine("Copies=1");
            sb.AppendLine("DocumentTitle=PDFCreator Testpage");
            sb.AppendLine("");

            return sb.ToString();
        }

        private string GetPsFileContent()
        {
            var sb = new StringBuilder(Testpage.TestPage);
            sb.Replace("[INFOEDITION]", _applicationNameProvider.EditionName.ToUpper());
            sb.Replace("[INFOTITLE]", "PDFCreator " + _versionHelper.ApplicationVersion);
            sb.Replace("[INFODATE]", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.Replace("[INFOAUTHORS]", "pdfforge");
            sb.Replace("[INFOHOMEPAGE]", Urls.PdfforgeWebsiteUrl);
            sb.Replace("[INFOPDFCREATOR]", "PDFCreator " + _versionHelper.ApplicationVersion);

            sb.Replace("[INFOCOMPUTER]", Environment.MachineName);
            sb.Replace("[INFOWINDOWS]", _osHelper.GetWindowsVersion());
            sb.Replace("[INFO64BIT]", _osHelper.Is64BitOperatingSystem.ToString());

            return sb.ToString();
        }
    }
}
