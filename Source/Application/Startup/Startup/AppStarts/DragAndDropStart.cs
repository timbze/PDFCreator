using pdfforge.PDFCreator.Core.Controller;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class DragAndDropStart : MaybePipedStart
    {
        private readonly IFileConversionAssistant _fileConversionAssistant;

        public DragAndDropStart(IFileConversionAssistant fileConversionAssistant, IMaybePipedApplicationStarter maybePipedApplicationStarter)
            : base(maybePipedApplicationStarter)
        {
            _fileConversionAssistant = fileConversionAssistant;
        }

        public ICollection<string> DroppedFiles { get; set; }

        protected override string ComposePipeMessage()
        {
            return "DragAndDrop|" + string.Join("|", DroppedFiles);
        }

        protected override bool StartApplication()
        {
            _fileConversionAssistant.HandleFileList(DroppedFiles);
            return true;
        }
    }
}
