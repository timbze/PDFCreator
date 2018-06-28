using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public abstract class ADebugSettingsItemControlModel : TranslatableViewModelBase<DebugSettingsTranslation>
    {
        protected readonly ISettingsManager SettingsManager;
        public ICurrentSettingsProvider SettingsProvider { get; private set; }
        
        protected ADebugSettingsItemControlModel(ISettingsManager settingsManager, ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings):base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            SettingsManager = settingsManager;
            SettingsProvider = settingsProvider;
        }

        public IGpoSettings GpoSettings { get; private set; }

        public event EventHandler<SettingsEventArgs> SettingsLoaded;

        protected void ApplySettingsProcedure(PdfCreatorSettings settings)
        {
            SettingsManager.ApplyAndSaveSettings(settings);
            SettingsManager.LoadAllSettings(); //Load settings to ensure default profile
            SettingsManager.SaveCurrentSettings(); //Save settings again to synch registry with current settings
            SettingsLoaded?.Invoke(this, new SettingsEventArgs(SettingsProvider.Settings));
            SettingsProvider.Reset();
        }

        public class SettingsEventArgs : EventArgs
        {
            public SettingsEventArgs(PdfCreatorSettings settings)
            {
                Settings = settings;
            }

            public PdfCreatorSettings Settings { get; private set; }
        }
    }
}
