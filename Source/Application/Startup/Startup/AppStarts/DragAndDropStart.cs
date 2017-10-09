using pdfforge.PDFCreator.Core.Controller;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class DragAndDropStart : MaybePipedStart
    {
        private readonly IFileConversionHandler _fileConversionHandler;

        public DragAndDropStart(IFileConversionHandler fileConversionHandler, IMaybePipedApplicationStarter maybePipedApplicationStarter)
            : base(maybePipedApplicationStarter)
        {
            _fileConversionHandler = fileConversionHandler;
        }

        public ICollection<string> DroppedFiles { get; set; }

        protected override string ComposePipeMessage()
        {
            return "DragAndDrop|" + string.Join("|", DroppedFiles);
        }

        protected override bool StartApplication()
        {
            _fileConversionHandler.HandleFileList(DroppedFiles);
            return true;
        }
    }
}
