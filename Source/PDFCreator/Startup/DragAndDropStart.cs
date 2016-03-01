using System.Collections.Generic;
using NLog;
using pdfforge.PDFCreator.Helper;

namespace pdfforge.PDFCreator.Startup
{
    internal class DragAndDropStart : MaybePipedStart
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ICollection<string> DroppedFiles { get; private set; }

        public DragAndDropStart(ICollection<string> droppedFiles)
        {
            _logger.Debug("Launched Drag & Drop");
            DroppedFiles = droppedFiles;
        }

        internal override string ComposePipeMessage()
        {
            return "DragAndDrop|" + string.Join("|", DroppedFiles);
        }

        internal override bool StartApplication()
        {
            DragAndDropHelper.PrintPrintableFiles(DroppedFiles);
            DragAndDropHelper.AddFilesToJobInfoQueue(DroppedFiles);
            return true;
        }
    }
}