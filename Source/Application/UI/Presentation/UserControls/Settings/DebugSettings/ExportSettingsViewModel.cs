using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class ExportSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly IIniSettingsAssistant _iniSettingsAssistant;

        public ExportSettingsViewModel(ISettingsManager settingsManager, ITranslationUpdater translationUpdater, IIniSettingsAssistant iniSettingsAssistant, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings) : base(settingsManager, translationUpdater, settingsProvider, gpoSettings)
        {
            _iniSettingsAssistant = iniSettingsAssistant;

            LoadIniSettingsCommand = new DelegateCommand(LoadIniSettingsExecute);
            SaveIniSettingsCommand = new DelegateCommand(SaveIniSettingsExecute);
        }

        public ICommand LoadIniSettingsCommand { get; }
        public ICommand SaveIniSettingsCommand { get; }

        public bool ProfileManagementIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableProfileManagement;
            }
        }

        private void LoadIniSettingsExecute(object o)
        {
            var success = _iniSettingsAssistant.LoadIniSettings();

            if (!success)
                return;

            ApplySettingsProcedure(SettingsProvider.Settings);
        }

        private void SaveIniSettingsExecute(object o)
        {
            _iniSettingsAssistant.SaveIniSettings(ApplicationSettings);
        }
    }
}
