using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class SettingsUpgradeHelper
    {
        private readonly int _targetVersion;

        public SettingsUpgradeHelper(int targetVersion)
        {
            _targetVersion = targetVersion;
        }

        public SettingsUpgradeHelper()
        {
            _targetVersion = new ApplicationProperties().SettingsVersion;
        }

        public void UpgradeSettings(Data settingsData)
        {
            var upgrader = new SettingsUpgrader(settingsData);

            if (upgrader.RequiresUpgrade(_targetVersion))
                upgrader.Upgrade(_targetVersion);
        }
    }
}
