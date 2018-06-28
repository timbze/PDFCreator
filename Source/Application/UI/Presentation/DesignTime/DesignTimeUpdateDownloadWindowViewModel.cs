using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateDownloadWindowViewModel : UpdateDownloadWindowViewModel
    {
        public DesignTimeUpdateDownloadWindowViewModel() : base(null, null, null, new DesignTimeTranslationUpdater(), null, null)
        {
            ProgressPercentage = 42;
            DownloadSpeedText = "450 MB/s";
        }
    }
}
