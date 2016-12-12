using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeApplicationSettingsViewModel : ApplicationSettingsViewModel
    {
        public DesignTimeApplicationSettingsViewModel() : base(new ApplicationSettingsViewModelBundle(new DesignTimeGeneralTabViewModel(), new DesignTimePrinterTabViewModel(), new DesignTimeTitleTabViewModel(), new DesignTimeDebugTabViewModel(), new DesignTimeLicenseTabViewModel(), new DesignTimePdfArchitectTabViewModel()), new TranslationHelper(new TranslationProxy(), new DefaultSettingsProvider(), new AssemblyHelper()), new LicenseOptionProvider(false))
        {
        }
    }
}