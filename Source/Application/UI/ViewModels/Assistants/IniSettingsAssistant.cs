using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
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
        private readonly ITranslator _translator;

        public IniSettingsAssistant(IInteractionInvoker interactionInvoker, ITranslator translator, ISettingsManager settingsManager, IDataStorageFactory dataStorageFactory, IIniSettingsLoader iniSettingsLoader)
        {
            _interactionInvoker = interactionInvoker;
            _translator = translator;
            _settingsManager = settingsManager;
            _settingsProvider = settingsManager.GetSettingsProvider();
            _dataStorageFactory = dataStorageFactory;
            _iniSettingsLoader = iniSettingsLoader;
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
            var message = _translator.GetTranslation("ApplicationSettingsWindow", "InvalidSettingsWarning");
            var caption = _translator.GetTranslation("ApplicationSettingsWindow", "InvalidSettings");

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            _interactionInvoker.Invoke(interaction);
        }

        private bool QueryOverwriteSettings()
        {
            var message = _translator.GetTranslation("ApplicationSettingsWindow", "LoadSettingsFromFileWarning");
            var caption = _translator.GetTranslation("ApplicationSettingsWindow", "OverwriteAllSettings");

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        private string QueryLoadFileName()
        {
            var interaction = new OpenFileInteraction();

            interaction.Filter = _translator.GetTranslation("ApplicationSettingsWindow", "IniFileFilter");
            interaction.FileName = "PDFCreator.ini";

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }

        private string QuerySaveFileName()
        {
            var interaction = new SaveFileInteraction();
            interaction.Filter = _translator.GetTranslation("ApplicationSettingsWindow", "IniFileFilter");
            interaction.FileName = "PDFCreator.ini";

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }
    }
}