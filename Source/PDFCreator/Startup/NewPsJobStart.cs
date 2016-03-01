using System.Runtime.InteropServices;
using NLog;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Startup
{
    internal class NewPsJobStart : MaybePipedStart
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int WTSGetActiveConsoleSessionId();
       
        public string NewPsFile { get; private set; }
        public string PrinterName { get; private set; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private string _newInfFile;
        private string NewInfFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_newInfFile))
                    return _newInfFile;

                var spool = JobInfoQueue.Instance.SpoolFolder;
                _newInfFile = PsFileHelper.TransformToInfFile(NewPsFile, spool, PrinterName);

                if (string.IsNullOrEmpty(_newInfFile))
                    _newInfFile = "";

                return _newInfFile;
            }
        }

        public NewPsJobStart(string newPsFile, string printerName = "PDFCreator")
        {
            NewPsFile = newPsFile;
            PrinterName = printerName;
        }

        internal override string ComposePipeMessage()
        {
            return "NewJob|" + NewInfFile;
        }

        internal override bool StartApplication()
        {
            if (string.IsNullOrEmpty(NewInfFile))
                return false;

            _logger.Debug("Adding new job.");
            JobInfoQueue.Instance.Add(NewInfFile);

            return true;
        }
    }
}
