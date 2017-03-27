using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.UserControls.ApplicationSettings
{
    public class DesignTimeGeneralTabViewModel : GeneralTabViewModel
    {
        public DesignTimeGeneralTabViewModel()
            : base(new DesignTimeLanguageProvider(), null,
                new UpdateAssistant(null, null, null, null, null, new InstallationPathProvider("", "", ""), null, new DesignTimeUpdateInfoProvider()), null, null, null, null, new GeneralTabTranslation())
        {
        }
    }

    public class DesignTimeUpdateInfoProvider : UpdateInformationProvider
    {
        public DesignTimeUpdateInfoProvider() : base("", "")
        {
        }
    }
}