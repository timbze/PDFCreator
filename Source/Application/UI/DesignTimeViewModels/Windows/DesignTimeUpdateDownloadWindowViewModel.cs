using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeUpdateDownloadWindowViewModel : UpdateDownloadWindowViewModel
    {
        public DesignTimeUpdateDownloadWindowViewModel() : base(null, null, null, new UpdateDownloadWindowTranslation())
        {
            ProgressPercentage = 42;
            DownloadSpeedText = "450 MB/s";
        }
    }
}