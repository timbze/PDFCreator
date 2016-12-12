using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeUpdateDownloadWindowViewModel : UpdateDownloadWindowViewModel
    {
        public DesignTimeUpdateDownloadWindowViewModel() : base(null, null, null)
        {
            ProgressPercentage = 42;
            DownloadSpeedText = "450 MB/s";
        }
    }
}