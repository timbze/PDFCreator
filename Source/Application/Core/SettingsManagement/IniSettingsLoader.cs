namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IIniSettingsLoader
    {
        Conversion.Settings.PdfCreatorSettings LoadIniSettings(string iniFile);
    }

    public class IniSettingsLoader : IIniSettingsLoader
    {
        private readonly IDataStorageFactory _dataStorageFactory;

        public IniSettingsLoader(IDataStorageFactory dataStorageFactory)
        {
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
