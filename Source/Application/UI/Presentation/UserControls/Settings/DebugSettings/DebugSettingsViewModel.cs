using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class DebugSettingsViewModel : TranslatableViewModelBase<DebugSettingsTranslation>, ITabViewModel
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IGpoSettings _gpoSettings;
        public string Title => Translation.DebugTabTitle;
        public IconList Icon => IconList.DebugSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => DebugIsDisabled;

        public DebugSettingsViewModel(ITranslationUpdater translationUpdater, ISettingsProvider settingsProvider, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _settingsProvider = settingsProvider;
            _gpoSettings = gpoSettings;
        }


        public bool DebugIsDisabled
        {
            get
            {
                if (_settingsProvider.Settings.ApplicationSettings == null)
                    return false;

                return _gpoSettings != null ? _gpoSettings.DisableDebugTab : false;
            }
        }
    }
}
