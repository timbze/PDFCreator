using System.Collections.Generic;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeFileConversionHandler : IFileConversionHandler
    {
        public void HandleFileList(IEnumerable<string> droppedFiles)
        {
        }
    }

    public class DesignTimeDragAndDropHandler : DragAndDropEventHandler
    {
        public DesignTimeDragAndDropHandler() : base(new DesignTimeFileConversionHandler())
        {
        }
    }
}