using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeConvertTabViewModel : ConvertTabViewModel
    {
        public DesignTimeConvertTabViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }

    internal class DesignTimePngTabViewModel : ConvertPngViewModel
    {
        public DesignTimePngTabViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Png;
        }
    }

    internal class DesignTimeConvertJpgViewModel : ConvertJpgViewModel
    {
        public DesignTimeConvertJpgViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Jpeg;
        }
    }

    internal class DesignTimeConvertTiffViewModel : ConvertTiffViewModel
    {
        public DesignTimeConvertTiffViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
            CurrentProfile.OutputFormat = OutputFormat.Tif;
        }
    }
}
