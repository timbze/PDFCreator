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
        protected readonly ICurrentSettingsProvider SettingsProvider;
        
        protected ADebugSettingsItemControlModel(ISettingsManager settingsManager, ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings):base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            SettingsManager = settingsManager;
            SettingsProvider = settingsProvider;
            ApplicationSettings = settingsProvider.Settings.ApplicationSettings;
        }

        public IGpoSettings GpoSettings { get; private set; }

        public ApplicationSettings ApplicationSettings { get; private set; }
 
        public event EventHandler<SettingsEventArgs> SettingsLoaded;

        protected void ApplySettingsProcedure(PdfCreatorSettings settings)
        {
            SettingsManager.ApplyAndSaveSettings(settings);
            SettingsManager.LoadPdfCreatorSettings(); //Load settings to ensure default profile
            SettingsManager.SaveCurrentSettings(); //Save settings again to synch registry with current settings
            SettingsLoaded?.Invoke(this, new SettingsEventArgs(SettingsProvider.Settings));
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
