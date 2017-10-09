using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class PdfCreatorAdapter
    {
        private readonly IDirectConversionProvider _directConversionProvider;
        private readonly IFile _file;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IPathSafe _pathSafe;
        private readonly IPipeServerManager _pipeServerManager;
        private readonly PrintFileHelperComFactory _printFileHelperComFactory;
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly ISpoolFolderAccess _spoolFolderAccess;

        public PdfCreatorAdapter(IFile file, IPathSafe pathSafe, PrintFileHelperComFactory printFileHelperComFactory, IJobInfoQueue jobInfoQueue, ISpoolerProvider spoolerProvider, ISpoolFolderAccess spoolFolderAccess, IJobInfoManager jobInfoManager, IDirectConversionProvider directConversionProvider, IPrinterHelper printerHelper, IPipeServerManager pipeServerManager)
        {
            PrinterHelper = printerHelper;
            _file = file;
            _pathSafe = pathSafe;
            _printFileHelperComFactory = printFileHelperComFactory;
            _jobInfoQueue = jobInfoQueue;
            _spoolerProvider = spoolerProvider;
            _spoolFolderAccess = spoolFolderAccess;
            _jobInfoManager = jobInfoManager;
            _directConversionProvider = directConversionProvider;
            _pipeServerManager = pipeServerManager;
        }

        public IPrinterHelper PrinterHelper { get; private set; }

        public bool IsInstanceRunning => _pipeServerManager.IsServerRunning();

        public void PrintFile(string path)
        {
            PrintFileSwitchingPrinters(path, false);
        }

        public void PrintFileSwitchingPrinters(string path, bool allowDefaultPrinterSwitch)
        {
            PathCheck(path);

            var printFileHelper = _printFileHelperComFactory.CreatePrintFileHelperCom();

            printFileHelper.AddFile(path);
            printFileHelper.AllowDefaultPrinterSwitch = allowDefaultPrinterSwitch;
            printFileHelper.PrintAll();
        }

        public void AddFileToQueue(string path)
        {
            PathCheck(path);

            var fileExtension = _pathSafe.GetExtension(path) ?? string.Empty;
            var legalFileTypes = new List<string> { ".ps", ".pdf" };

            fileExtension = fileExtension.ToLowerInvariant();
            if (!legalFileTypes.Contains(fileExtension))
                throw new COMException("Only .ps and .pdf files can be directly added to the queue.");

            var spoolFolder = _spoolerProvider.SpoolFolder;

            if (!_spoolFolderAccess.CanAccess())
                throw new COMException("Accessing the spool folder failed.");

            var isPdf = fileExtension.EndsWith(".pdf");
            var fileHelper = isPdf ? _directConversionProvider.GetPdfConversion() : _directConversionProvider.GetPsConversion();

            var infFile = fileHelper.TransformToInfFile(path, spoolFolder);

            _jobInfoQueue.Add(_jobInfoManager.ReadFromInfFile(infFile));
        }

        private void PathCheck(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new COMException("The specified path must not be empty or uninitiliazed.");

            if (!_file.Exists(path))
                throw new COMException("File with such a path doesn't exist. Please check if the specified path is correct.");
        }
    }
}
