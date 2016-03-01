using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.PrintFile;

namespace pdfforge.PDFCreator.COM
{
    internal class PrintFileHelperCom : PrintFileHelperBase
    {
        public bool AllowDefaultPrinterSwitch { get; set; }

        protected override void DirectoriesNotSupportedHint()
        {
            throw new COMException("Directories cannot be printed!");
        }

        protected override void UnprintableFilesHint(IList<PrintCommand> unprintable)
        {
            StringBuilder unprintableFilesNames = new StringBuilder("The following file cannot be printed: \n");

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
