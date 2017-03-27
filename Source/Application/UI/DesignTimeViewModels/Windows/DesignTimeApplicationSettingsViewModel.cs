using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeApplicationSettingsViewModel : ApplicationSettingsViewModel
    {
        public DesignTimeApplicationSettingsViewModel() : 
            base(new ApplicationSettingsViewModelBundle(new DesignTimeGeneralTabViewModel(), new DesignTimePrinterTabViewModel(), new DesignTimeTitleTabViewModel(), new DesignTimeDebugTabViewModel(), new DesignTimeLicenseTabViewModel(), new DesignTimePdfArchitectTabViewModel()), new TranslationHelper(new DefaultSettingsProvider(), new AssemblyHelper(), new TranslationFactory()), new LicenseOptionProvider(false), new ApplicationSettingsWindowTranslation())
        {
        }
    }
}