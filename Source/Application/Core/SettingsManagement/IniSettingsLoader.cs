namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IIniSettingsLoader
    {
        Conversion.Settings.PdfCreatorSettings LoadIniSettings(string iniFile);
    }

    public class IniSettingsLoader : IIniSettingsLoader
    {
        private readonly IDataStorageFactory _dataStorageFactory;
        private readonly ISettingsProvider _settingsProvider;

        public IniSettingsLoader(ISettingsManager settingsManager, IDataStorageFactory dataStorageFactory)
        {
            _settingsProvider = settingsManager.GetSettingsProvider();
            _dataStorageFactory = dataStorageFactory;
        }

        public Conversion.Settings.PdfCreatorSettings LoadIniSettings(string iniFile)
        {
            if (string.IsNullOrWhiteSpace(iniFile))
                return null;

            var iniStorage = _dataStorageFactory.BuildIniStorage();

            var settingsUpgrader = new SettingsUpgradeHelper();

            var settings = new DefaultSettingsBuilder().CreateEmptySettings(iniStorage);
            settings.LoadData(iniStorage, iniFile, settingsUpgrader.UpgradeSettings);

            return settings;
        }
    }
}
