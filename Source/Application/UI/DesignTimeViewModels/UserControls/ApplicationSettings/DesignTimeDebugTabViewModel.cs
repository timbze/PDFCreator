using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeDebugTabViewModel : DebugTabViewModel
    {
        public DesignTimeDebugTabViewModel()
            : base(new DesignTimeSettingsManager(), null, null, null, null, null, null, new DebugTabTranslation())
        {
            LoggingHelper.InitConsoleLogger("DesignTime", LoggingLevel.Off);
        }
    }
}