using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateDownloadWindowTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "_Cancel";
        public string InstallHintText { get; private set; } = "The update will be started when you close PDFCreator";
        public string LoadingUpdateText { get; private set; } = "Loading Update...";
        public string Title { get; private set; } = "PDFCreator Plus Updater";
    }
}
