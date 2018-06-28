using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IIniSettingsAssistant
    {
        bool LoadIniSettings();

        void SaveIniSettings(ApplicationSettings settings);
    }

    public class IniSettingsAssistant : IIniSettingsAssistant
    {
        private readonly IDataStorageFactory _dataStorageFactory;
        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsProvider _settingsProvider;
        private LoadSettingsTranslationTranslation _translation;

        public IniSettingsAssistant(IInteractionInvoker interactionInvoker, ITranslationUpdater translationUpdater, ISettingsManager settingsManager, IDataStorageFactory dataStorageFactory, IIniSettingsLoader iniSettingsLoader)
        {
            _interactionInvoker = interactionInvoker;
            _settingsManager = settingsManager;
            _settingsProvider = settingsManager.GetSettingsProvider();
            _dataStorageFactory = dataStorageFactory;
            _iniSettingsLoader = iniSettingsLoader;

            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
        }

        public bool LoadIniSettings()
        {
            var fileName = QueryLoadFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var overwriteSettings = QueryOverwriteSettings();
            if (!overwriteSettings)
                return false;

            var settings = _iniSettingsLoader.LoadIniSettings(fileName);

            if (!_settingsProvider.CheckValidSettings(settings))
            {
                DisplayInvalidSettingsWarning();
                return false;
            }

            _settingsManager.ApplyAndSaveSettings(settings);

            return true;
        }

        //TODO Why do we pass appSettings here? Most likely they are already in sync
        public void SaveIniSettings(ApplicationSettings appSettings)
        {
            var fileName = QuerySaveFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var ini = _dataStorageFactory.BuildIniStorage();
            var settings = _settingsProvider.Settings.Copy();
            settings.ApplicationSettings = appSettings;
            settings.SaveData(ini, fileName);
        }

        private void DisplayInvalidSettingsWarning()
        {
            var message = _translation.InvalidSettingsWarning;
            var caption = _translation.InvalidSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            _interactionInvoker.Invoke(interaction);
        }

        private bool QueryOverwriteSettings()
        {
            var message = _translation.LoadSettingsFromFileWarning;
            var caption = _translation.OverwriteAllSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        private string QueryLoadFileName()
        {
            var interaction = new OpenFileInteraction();

            interaction.Filter = _translation.IniFileFilter;
            interaction.FileName = "PDFCreator.ini";

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }

        private string QuerySaveFileName()
        {
            var interaction = new SaveFileInteraction();
            interaction.Filter = _translation.IniFileFilter;
            interaction.FileName = "PDFCreator.ini";

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }
    }

    public class LoadSettingsTranslationTranslation : ITranslatable
    {
        public string IniFileFilter { get; private set; } = "INI files (*.ini)|*.ini";
        public string InvalidSettings { get; private set; } = "Invalid settings";
        public string InvalidSettingsWarning { get; private set; } = "The file does not appear to contain valid PDFCreator settings.";
        public string LoadSettingsFromFileWarning { get; private set; } = "By loading the file all settings and profiles will be overwritten. Do you want to continue?";
        public string OverwriteAllSettings { get; private set; } = "Overwrite all settings";
    }
}
