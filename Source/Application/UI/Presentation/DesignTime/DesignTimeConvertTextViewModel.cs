using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeConvertTextViewModel : ConvertTextViewModel
    {
        public DesignTimeConvertTextViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
            CurrentProfile.OutputFormat = OutputFormat.Txt;
        }
    }
}
