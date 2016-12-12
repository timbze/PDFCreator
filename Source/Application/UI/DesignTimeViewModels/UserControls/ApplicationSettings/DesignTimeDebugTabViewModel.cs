using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeDebugTabViewModel : DebugTabViewModel
    {
        public DesignTimeDebugTabViewModel()
            : base(new TranslationProxy(), new DesignTimeSettingsManager(), null, null, null, null, null, null)
        {
            LoggingHelper.InitConsoleLogger("DesignTime", LoggingLevel.Off);
        }
    }
}