using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class PrintFileHelperComFactory
    {
        private readonly IFileAssoc _fileAssoc;
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;

        public PrintFileHelperComFactory(IPrinterHelper printerHelper, ISettingsProvider settingsProvider,
            IFileAssoc fileAssoc)
        {
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            _fileAssoc = fileAssoc;
        }

        public PrintFileHelperCom CreatePrintFileHelperCom()
        {
            return new PrintFileHelperCom(_printerHelper, _settingsProvider, _fileAssoc);
        }
    }

    public class PrintFileHelperCom : PrintFileHelperBase
    {
        internal PrintFileHelperCom(IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IFileAssoc fileAssoc) : base(printerHelper, settingsProvider, fileAssoc)
        {
        }

        public bool AllowDefaultPrinterSwitch { get; set; }

        protected override void DirectoriesNotSupportedHint()
        {
            throw new COMException("Directories cannot be printed!");
        }

        protected override bool UnprintableFilesQuery(IList<PrintCommand> unprintable)
        {
            var unprintableFilesNames = new StringBuilder("The following file cannot be printed: \n");

            foreach (var unprintableFile in unprintable)
            {
                unprintableFilesNames.AppendLine(unprintableFile.Filename);
            }

            throw new COMException(unprintableFilesNames.ToString());
        }

        protected override bool QuerySwitchDefaultPrinter()
        {
            //Depending on what the COM user chose to do, we set the
            //default printer or not 
            return AllowDefaultPrinterSwitch;
        }
    }
}
